var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app
    .UseSwagger();

app
    .UseSwaggerUI();

app
    .MapGet("/get-config", (IConfiguration config) =>
    {
        return Results.Ok
        (
            config
                .AsEnumerable()
                .Where(kv => kv.Value != null)
                .ToDictionary(kv=> kv.Key, kv=> kv.Value)
        );
    })
    .WithName("GetConfig")
    .WithOpenApi();

await app.RunAsync();
