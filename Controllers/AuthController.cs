using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlowAPI.Data;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        var existingUser = _context.Users
            .FirstOrDefault(x => x.Email == user.Email);

        if (existingUser != null)
            return BadRequest(new
            {
                message = "Email already exists"
            });

        user.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully"
        });
    }

    [HttpPost("login")]
    public IActionResult Login(User loginUser)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == loginUser.Email);

        if (user == null)
            return Unauthorized(new
            {
                message = "Invalid credentials"
            });

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            loginUser.PasswordHash,
            user.PasswordHash
        );

        if (!validPassword)
            return Unauthorized(new
            {
                message = "Invalid credentials"
            });

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler()
                .WriteToken(token)
        });
    }
}