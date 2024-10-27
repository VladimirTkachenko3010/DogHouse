using Application.DTOs;
using Application.Utilities;

namespace Application.Interfaces
{
    public interface IDogService
    {
        Task<Result> CreateDogAsync(DogCreateDto dogDto);
        Task<IEnumerable<DogDto>> GetDogsAsync(DogQueryParameters parameters);
    }
}
