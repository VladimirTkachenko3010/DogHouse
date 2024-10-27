using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDogRepository
    {
        IQueryable<Dog> Query();
        Task<IEnumerable<Dog>> GetAllAsync();
        Task<Dog> GetByIdAsync(int id);
        Task<bool> DogExistsAsync(string name);
        Task AddAsync(Dog dog);
        Task SaveChangesAsync();
    }
}
