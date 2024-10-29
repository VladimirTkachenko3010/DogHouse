using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests
{
    public class DogRepositoryTests
    {
        private readonly DbContextOptions<DogContext> _dbContextOptions;

        public DogRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DogContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // We generate a unique database for each test
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDogs()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                context.Dogs.AddRange(new Dog { Name = "Buddy", Color = "Brown" }, new Dog { Name = "Max", Color = "Black" });
                await context.SaveChangesAsync();

                var repository = new DogRepository(context);

                var dogs = await repository.GetAllAsync();

                Assert.Equal(2, dogs.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_WhenDogExists_ShouldReturnDog()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                var dog = new Dog { Name = "Buddy", Color = "Brown" };
                context.Dogs.Add(dog);
                await context.SaveChangesAsync();

                var repository = new DogRepository(context);

                var retrievedDog = await repository.GetByIdAsync(dog.Id);

                Assert.NotNull(retrievedDog);
                Assert.Equal(dog.Name, retrievedDog.Name);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WhenDogDoesNotExist_ShouldReturnNull()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                var repository = new DogRepository(context);

                var retrievedDog = await repository.GetByIdAsync(999);

                Assert.Null(retrievedDog);
            }
        }

        [Fact]
        public async Task DogExistsAsync_WhenDogExists_ShouldReturnTrue()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                var dog = new Dog { Name = "Buddy", Color = "Brown" };
                context.Dogs.Add(dog);
                await context.SaveChangesAsync();

                var repository = new DogRepository(context);

                var exists = await repository.DogExistsAsync(dog.Name);

                Assert.True(exists);
            }
        }

        [Fact]
        public async Task DogExistsAsync_WhenDogDoesNotExist_ShouldReturnFalse()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                var repository = new DogRepository(context);

                var exists = await repository.DogExistsAsync("NonExistentName");

                Assert.False(exists);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddDog()
        {
            using (var context = new DogContext(_dbContextOptions))
            {
                var repository = new DogRepository(context);
                var dog = new Dog { Name = "Buddy", Color = "Brown" };

                await repository.AddAsync(dog);
                await repository.SaveChangesAsync();

                Assert.Equal(1, context.Dogs.Count());
                Assert.Equal("Buddy", context.Dogs.First().Name);
            }
        }
    }

}
