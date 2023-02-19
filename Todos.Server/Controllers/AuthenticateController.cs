using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Todos.Server.Entities;
using Todos.Server.Models.Authenticate;

namespace Todos.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]

public class AuthenticateController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _db;

    public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext db)  
    {  
            _userManager = userManager;
            _configuration = configuration;
            _db = db;
    }

    private JwtSecurityToken CreateJwtWithClaims(string userName)
    {
        var authClaims = new List<Claim>  
        {  
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));  
  
        var token = new JwtSecurityToken(  
            _configuration["JWT:ValidIssuer"],  
            _configuration["JWT:ValidAudience"],  
            expires: DateTime.Now.AddHours(3),  
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)  
    {  
        var user = await _userManager.FindByNameAsync(model.Name);  
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))  
        {
            var token = CreateJwtWithClaims(user.UserName);
            
            await _db.Categories.AddAsync(new Category()
            {
                Name = "all",
                ApplicationUser = user
            });

            await _db.SaveChangesAsync();

            return Ok(new  
            {  
                access_token = new JwtSecurityTokenHandler().WriteToken(token),  
                expiration = token.ValidTo  
            });  
        }  
        
        return Unauthorized();  
    }  
  
    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)  
    {  
        var userExists = await _userManager.FindByNameAsync(model.Name);
        if (userExists != null)  
            return StatusCode(StatusCodes.Status500InternalServerError, "User already exists!");  
  
        var user = new ApplicationUser()  
        {
            SecurityStamp = Guid.NewGuid().ToString(),  
            UserName = model.Name  
        };

        var result = await _userManager.CreateAsync(user, model.Password);  
            
        if (!result.Succeeded)  
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "User creation failed! Please check user details and try again.");

        var token = CreateJwtWithClaims(user.UserName);

        return Ok(new  
        {  
            access_token = new JwtSecurityTokenHandler().WriteToken(token),  
            expiration = token.ValidTo  
        }); 
    }
}