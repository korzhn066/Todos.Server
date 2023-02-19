using Microsoft.AspNetCore.Identity;

namespace Todos.Server.Entities;

public class ApplicationUser : IdentityUser
{
    public List<Category> Categories { get; set; } = new();
    public List<Todo> Todos { get; set; } = new();
}