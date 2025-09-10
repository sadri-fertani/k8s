using Microsoft.AspNetCore.DataProtection;
using MonAppK8s.Components;
using MonAppK8s.Services;
using Polly;
using Refit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(builder =>
{    
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();


// Add services to the container.
var redisConfig = new ConfigurationOptions
{
    EndPoints = { builder.Configuration["REDIS_URL"]! },
    Password = builder.Configuration["REDIS_KEY"]!,         // si Redis est protégé par mot de passe
    Ssl = false,                                            // à activer si Redis est configuré en SSL
    AbortOnConnectFail = false,                             // utile en environnement distribué
    ConnectTimeout = 10000,                                  // timeout en ms
    ClientName = "MonAppK8s",
    KeepAlive = 180
};

// Retry 3 fois avec 2 secondes d’intervalle
var lazyMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
{
    logger.LogInformation("Lazy initialization of Redis...");

    return Policy
        .Handle<RedisConnectionException>()
        .WaitAndRetry(3, i => TimeSpan.FromSeconds(2), (exception, timeSpan, retryCount, context) =>
        {
            logger.LogWarning("Attempt {retryCount} failed: {Message}", retryCount, exception.Message);
        })
        .Execute(() =>
        {
            logger.LogInformation("Connecting to Redis...");
            var conn = ConnectionMultiplexer.Connect(redisConfig);
            logger.LogInformation("Redis connection established.");
            return conn;
        });
});

// Enregistrement du multiplexer pour DI
builder.Services.AddSingleton<IConnectionMultiplexer>(lazyMultiplexer.Value);

builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis
    (
        lazyMultiplexer.Value,
        "DataProtection-Keys"
    )
    .SetApplicationName("MonAppK8s");

builder.Services
    .AddRefitClient<IMonApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["API_URL"]!));

builder.Services
    .AddRefitClient<IMonApi2>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["API2_URL"]!));

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
