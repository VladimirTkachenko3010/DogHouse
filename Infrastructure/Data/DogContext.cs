using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DogContext : DbContext
    {
        public DogContext(DbContextOptions<DogContext> options) : base(options) { }

        public DbSet<Dog> Dogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dog>()
                .HasIndex(d => d.Name)
                .IsUnique();  // Unique index on the Name field
        }
    }
}
