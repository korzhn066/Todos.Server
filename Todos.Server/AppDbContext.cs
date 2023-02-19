using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todos.Server.Entities;

namespace Todos.Server;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Category> Categories { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    
}