using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace IBayApi2.Controller;

[Route("[controller]")]
[ApiController]

public class LoginController : ControllerBase
{
    private readonly ApiContext _context;

    private readonly IConfiguration _configuration;

    private readonly Hashpassword _hashpassword;

    
    public LoginController(ApiContext context, IConfiguration configuration,Hashpassword hashpassword)
    {
        _context = context;
        _configuration = configuration;
        _hashpassword = hashpassword;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Loginaccount([FromBody] User user)
    {
        var hashpassword = _hashpassword.Hpassword(user.password);

        var users =  _context.Users.FirstOrDefault(u => u.pseudo == user.pseudo && u.password == hashpassword);

        if (users == null)
        {
            return BadRequest();
        }
        
        var token = GenerateJwtToken(users);
        return Ok(token);
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User payload)
    {
        payload.password = _hashpassword.Hpassword(payload.password);
            
        await _context.Users.AddAsync(payload);

        await _context.SaveChangesAsync();

        return payload;
    }
    
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.pseudo),
            new Claim("UserId", user.Id.ToString())
        };

        if (!string.IsNullOrEmpty(user.role))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.role));
        }
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}