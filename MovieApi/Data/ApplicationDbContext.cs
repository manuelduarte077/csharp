using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Agregar las entidades (Models)
    public DbSet<Category> Categories { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
}