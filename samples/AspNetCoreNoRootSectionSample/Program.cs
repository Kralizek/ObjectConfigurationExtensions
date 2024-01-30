var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObject(new
{
    IsEnabled = false
});

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration.AsEnumerable().OrderBy(c => c.Key).ToDictionary(c => c.Key, v => v.Value));

app.Run();
