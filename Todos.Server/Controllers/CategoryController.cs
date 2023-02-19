using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todos.Server.DTO;
using Todos.Server.Entities;

namespace Todos.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CategoryController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveCategory(int categoryId)
    {
        var category = await _db.Categories.FindAsync(categoryId);

        if (category is null)
            return NotFound(); 
        
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPost]
    public async Task AddCategory(string categoryName)
    {
        var user = _userManager.FindByNameAsync(HttpContext.User.Identity!.Name).Result;

        await _db.Categories.AddAsync(new Category()
        {
            Name = categoryName,
            ApplicationUser = user
        });

        await _db.SaveChangesAsync();
    }

    [HttpGet]
    public IEnumerable<CategoryDto> GetAll()
    {
        var user =
            _db.Users.Include(u => u.Categories)
                .First(u => u.Id == _userManager.FindByNameAsync(HttpContext.User.Identity!.Name).Result.Id);

        var categories = new List<CategoryDto>();

        foreach (var category in user.Categories)
        {
            categories.Add(new CategoryDto()
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            });
        }

        return categories;
    }
}