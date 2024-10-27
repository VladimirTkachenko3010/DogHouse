using Application.DTOs;
using Application.Interfaces;
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
            var dogs = await _dogService.GetDogsAsync(parameters);
            return Ok(dogs);
        }

        [HttpPost("dog")]
        public async Task<IActionResult> CreateDog([FromBody] DogCreateDto dogDto)
        {
            var result = await _dogService.CreateDogAsync(dogDto);
            if (!result.IsSuccessful) return BadRequest(result.Errors);
            return CreatedAtAction(nameof(GetDogs), new { id = result.Id }, result);
        }
    }
}
