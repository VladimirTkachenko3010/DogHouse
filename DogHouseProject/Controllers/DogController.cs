using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class DogController : Controller
    {
        private readonly IDogService _dogService;
        public DogController(IDogService dogService)
        {
            _dogService = dogService;
        }


        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }

        [HttpGet("dogs")]
        public async Task<IActionResult> GetDogs([FromQuery] DogQueryParameters parameters)
        {
            var validator = new DogQueryParametersValidator();
            var validationResult = await validator.ValidateAsync(parameters);

            if (!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors
                    .Select(e => new
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    });

                return BadRequest(new { Errors = errorResponse });
            }

            var dogs = await _dogService.GetDogsAsync(parameters);
            return Ok(dogs);
        }

        [HttpPost("dog")]
        public async Task<IActionResult> CreateDog([FromBody] DogCreateDto dogDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Key != nameof(dogDto)) // Excluding dogDto from the list of errors
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x =>
                    {
                        // Check for deserialization error type (e.g. "could not be converted")
                        var errorMessage = x.Value.Errors.First().ErrorMessage;
                        if (errorMessage.Contains("could not be converted"))
                        {
                            // Extracting the field name and replacing it with a clear message
                            var fieldPath = x.Key.Replace("$.", ""); // Remove the "$." so that only the field path remains.
                            errorMessage = $"Invalid value type for field '{fieldPath}'. Expected a different type.";
                        }

                        return new
                        {
                            Field = x.Key.Replace("$.", ""),  // Чистое имя поля без "$."
                            Message = errorMessage
                        };
                    });

                return BadRequest(new { Errors = errors });
            }

            var result = await _dogService.CreateDogAsync(dogDto);
            if (!result.IsSuccessful) return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetDogs), new { id = result.Id }, result);
        }
    }
}
