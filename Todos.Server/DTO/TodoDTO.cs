namespace Todos.Server.DTO;

public class TodoDto
{
    public int TodoId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}