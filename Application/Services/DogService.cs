using Application.DTOs;
using Application.Interfaces;
using Application.Utilities;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository _dogRepository;

        public DogService(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        public async Task<Result> CreateDogAsync(DogCreateDto dogDto)
        {
            if (await _dogRepository.DogExistsAsync(dogDto.Name))
                return Result.Failure("Dog with the same name already exists.");

            var dog = MapToDog(dogDto);

            await _dogRepository.AddAsync(dog);
            await _dogRepository.SaveChangesAsync();

            return Result.Success(dog.Id);
        }

        public async Task<IEnumerable<DogDto>> GetDogsAsync(DogQueryParameters parameters)
        {
            var query = _dogRepository.Query();

            //Checking the correctness of the sorting parameters
            var validAttributes = new[] { "Name", "Color", "TailLength", "Weight" };
            if (!validAttributes.Contains(parameters.Attribute))
            {
                throw new ArgumentException("Invalid attribute for sorting.");
            }

            if (parameters.Order.ToLower() == "desc")
                query = query.OrderByDescending(d => EF.Property<object>(d, parameters.Attribute));
            else
                query = query.OrderBy(d => EF.Property<object>(d, parameters.Attribute));

            var dogs = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return dogs.Select(d => new DogDto
            {
                Name = d.Name,
                Color = d.Color,
                TailLength = d.TailLength,
                Weight = d.Weight
            });
        }

        public Dog MapToDog(DogCreateDto dto)
        {
            return new Dog
            {
                Name = dto.Name,
                Color = dto.Color,
                TailLength = dto.TailLength,
                Weight = dto.Weight
            };
        }
    }
}
