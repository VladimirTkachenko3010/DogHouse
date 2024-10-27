using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly DogContext _context;

        public DogRepository(DogContext context)
        {
            _context = context;
        }
        public IQueryable<Dog> Query()
        {
            return _context.Dogs.AsQueryable();
        }

        public async Task<IEnumerable<Dog>> GetAllAsync()
        {
            return await _context.Dogs.ToListAsync();
        }

        public async Task<Dog> GetByIdAsync(int id)
        {
            return await _context.Dogs.FindAsync(id);
        }

        public async Task<bool> DogExistsAsync(string name)
        {
            return await _context.Dogs.AnyAsync(d => d.Name == name);
        }

        public async Task AddAsync(Dog dog)
        {
            await _context.Dogs.AddAsync(dog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
