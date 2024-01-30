var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObjectWithNewtonsoftJson(new
{
    Text = "Something"
}, "Test");

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration.GetSection("Test").AsEnumerable().OrderBy(c => c.Key).ToDictionary(c => c.Key, v => v.Value));

app.Run();
