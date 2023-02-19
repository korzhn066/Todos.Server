using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todos.Server.DTO;
using Todos.Server.Entities;
using Todos.Server.Models;

namespace Todos.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]

public class TodoController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TodoController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveTodo(int todoId)
    {
        var todo = await _db.Todos.FindAsync(todoId);

        if (todo is null)
            return NotFound(); 
        
        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task AddTodo(TodoModel model)
    {
        var user = _userManager.FindByNameAsync(HttpContext.User.Identity!.Name).Result;
        
        await _db.Todos.AddAsync(new Todo()
        {
            Name = model.Name,
            Description = model.Description,
            Category = (await _db.Categories.FindAsync(model.CategoryId))!,
            ApplicationUser = user
        });

        await _db.SaveChangesAsync();
    }
    
    [HttpGet]
    public Task<IEnumerable<TodoDto>> GetByCategory(int categoryId)
    {
        var category =
            _db.Categories.Include(c => c.Todos).FirstAsync(c => c.CategoryId == categoryId).Result;
            
        var todos = new List<TodoDto>();

        foreach (var todo in category.Todos)
        {
            todos.Add(new TodoDto()
            {
                TodoId= todo.TodoId,
                Name = todo.Name,
                Description = todo.Description
            });
        }

        return Task.FromResult<IEnumerable<TodoDto>>(todos);
    }

    [HttpGet]
    public Task<IEnumerable<TodoDto>> GetAll()
    {
        var user =
            _db.Users.Include(u => u.Todos)
                .First(u => u.Id == _userManager.FindByNameAsync(HttpContext.User.Identity!.Name).Result.Id);

        var todos = new List<TodoDto>();

        foreach (var todo in user.Todos)
        {
            todos.Add(new TodoDto()
            {
                TodoId = todo.TodoId,
                Name = todo.Name,
                Description = todo.Description
            });
        }

        return Task.FromResult<IEnumerable<TodoDto>>(todos);
    }
}