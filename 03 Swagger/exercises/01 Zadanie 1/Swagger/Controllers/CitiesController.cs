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

            _logger.LogInformation($"Pobrano miasto: {city.Name}");
            return Ok(city);
        }

        /// <summary>
        /// Aktualizuje wybrane miasto
        /// </summary>
        /// <param name="id">ID miasta do aktualizacji</param>
        /// <param name="request">Nowe dane miasta</param>
        /// <returns>Zaktualizowane miasto</returns>
        [HttpPut("{id}", Name = "UpdateCity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] UpdateCityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Country))
            {
                return BadRequest("Nazwa i kraj nie mogą być puste");
            }

            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city == null)
            {
                return NotFound($"Miasto o ID {id} nie znaleziono");
            }

            city.Name = request.Name;
            city.Country = request.Country;

            _logger.LogInformation($"Zaktualizowano miasto: {city.Name}");
            return Ok(city);
        }

        /// <summary>
        /// Usuwa wybrane miasto
        /// </summary>
        /// <param name="id">ID miasta do usunięcia</param>
        /// <returns>Potwierdzenie usunięcia</returns>
        [HttpDelete("{id}", Name = "DeleteCity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var city = _cities.FirstOrDefault(c => c.Id == id);
            if (city == null)
            {
                return NotFound($"Miasto o ID {id} nie znaleziono");
            }

            _cities.Remove(city);
            _logger.LogInformation($"Usunięto miasto: {city.Name}");

            return Ok(new { message = $"Miasto '{city.Name}' zostało usunięte", deletedCity = city });
        }
    }

    public class CreateCityRequest
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class UpdateCityRequest
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
