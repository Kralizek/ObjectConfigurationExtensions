var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObject(new
{
    Value = 123,
    ManyValues = new ComplexObject[]
    {
        new ("New value", 234),
        new ("Another value", 345)
    },
    Flag = true,
    Text = "Something"
}, "Test");

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration.GetSection("Test").AsEnumerable().OrderBy(c => c.Key).ToDictionary(c => c.Key, v => v.Value));

app.Run();

public record ComplexObject(string Text, int Number);