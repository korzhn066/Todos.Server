namespace Todos.Server.Entities;

public class Todo
{
    public int TodoId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public ApplicationUser ApplicationUser { get; set; }
    public Category Category { get; set; }
}