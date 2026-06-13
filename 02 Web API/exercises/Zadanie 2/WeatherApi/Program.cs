using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

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

app.MapGet("/weather/{city}", async (string city, IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
var apiKey = config["OpenWeather:ApiKey"];
var httpClient = httpClientFactory.CreateClient();

var url =
    $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=pl";

var weather = await httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);

if (weather is null)
{
return Results.NotFound("Nie udało się pobrać pogody.");
}

return Results.Ok(new
{
Miasto = weather.Name,
Temperatura = weather.Main.Temp,
Odczuwalna = weather.Main.FeelsLike,
Wilgotnosc = weather.Main.Humidity,
Cisnienie = weather.Main.Pressure,
Opis = weather.Weather.FirstOrDefault()?.Description,
PredkoscWiatru = weather.Wind.Speed
});
});

app.Run();

public class OpenWeatherResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("main")]
    public MainInfo Main { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherInfo> Weather { get; set; }

    [JsonPropertyName("wind")]
    public WindInfo Wind { get; set; }
}

public class MainInfo
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
}

public class WeatherInfo
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class WindInfo
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}
