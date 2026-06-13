var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var directions = new[]
{
    "Północ",
    "Południe",
    "Wschód",
    "Zachód",
    "Północny-Wschód",
    "Północny-Zachód",
    "Południowy-Wschód",
    "Południowy-Zachód"
};

app.MapGet("/temperature", () =>
{
    var random = new Random();
    int temperature = random.Next(-20, 41);

    return Results.Ok(new
    {
        Temperature = temperature
    });
});

app.MapGet("/wind", () =>
{
    var random = new Random();
    string direction = directions[random.Next(directions.Length)];

    return Results.Ok(new
    {
        WindDirection = direction
    });
});

app.Run();
