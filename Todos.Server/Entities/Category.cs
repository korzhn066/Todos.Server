namespace Todos.Server.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    
    public ApplicationUser ApplicationUser { get; set; }
    public List<Todo> Todos { get; set; } = new();
}