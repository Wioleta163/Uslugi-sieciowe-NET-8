using Microsoft.AspNetCore.Mvc;
using Swagger.Models;

namespace Swagger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitiesController : ControllerBase
    {
        // Lista przechowywana w pamięci (statyczna kolekcja)
        private static List<City> _cities = new List<City>
        {
            new City { Id = 1, Name = "Warszawa", Country = "Polska" },
            new City { Id = 2, Name = "Kraków", Country = "Polska" },
            new City { Id = 3, Name = "Berlin", Country = "Niemcy" }
        };

        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ILogger<CitiesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Pobiera listę wszystkich miast
        /// </summary>
        /// <returns>Lista miast</returns>
        [HttpGet(Name = "GetAllCities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            _logger.LogInformation("Pobieranie listy miast");
            return Ok(_cities);
        }

        /// <summary>
        /// Dodaje nowe miasto do kolekcji
        /// </summary>
        /// <param name="city">Dane miasta do dodania</param>
        /// <returns>Dodane miasto</returns>
        [HttpPost(Name = "AddCity")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CreateCityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Country))
            {
                return BadRequest("Nazwa i kraj nie mogą być puste");
            }

            var newCity = new City
            {
                Id = _cities.Max(c => c.Id) + 1,
                Name = request.Name,
                Country = request.Country
            };

            _cities.Add(newCity);
            _logger.LogInformation($"Dodano nowe miasto: {newCity.Name}");

            return CreatedAtAction(nameof(Get), new { id = newCity.Id }, newCity);
        }

        /// <summary>
        /// Pobiera miasto po ID
        /// </summary>
        /// <param name="id">ID miasta</param>
        /// <returns>Miasto o podanym ID</returns>
        [HttpGet("{id}", Name = "GetCityById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city == null)
            {
                return NotFound($"Miasto o ID {id} nie znaleziono");
            }

            return Ok(city);
        }
    }

    public class CreateCityRequest
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
