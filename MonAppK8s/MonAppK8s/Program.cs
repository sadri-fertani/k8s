using Microsoft.AspNetCore.DataProtection;
using MonAppK8s.Components;
using MonAppK8s.Services;
using Refit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var redisConfig = new ConfigurationOptions
{
    EndPoints = { builder.Configuration["REDIS_URL"]! },
    Password = builder.Configuration["REDIS_KEY"]!,         // si Redis est protégé par mot de passe
    Ssl = false,                                            // à activer si Redis est configuré en SSL
    AbortOnConnectFail = false,                             // utile en environnement distribué
    ConnectTimeout = 5000,                                  // timeout en ms
    ClientName = "MonAppK8s"
};

var multiplexer = ConnectionMultiplexer.Connect(redisConfig);

// Enregistrement du multiplexer pour DI
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis
    (
        multiplexer,
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

app.Run();
