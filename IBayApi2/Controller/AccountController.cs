using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace IBayApi2.Controller;

[Route("")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApiContext _context;

    private readonly IConfiguration _configuration;

    private readonly Hashpassword _hashpassword;
    private readonly CartController _cartController;


    public AccountController(ApiContext context, IConfiguration configuration, Hashpassword hashpassword)
    {
        _context = context;
        _configuration = configuration;
        _hashpassword = hashpassword;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Loginaccount([FromBody] Member member)
    {
        try
        {
            var hashpassword = _hashpassword.Hpassword(member.Password);

            var users = _context.Member.FirstOrDefault(u => u.Pseudo == member.Pseudo && u.Password == hashpassword);

            if (users == null)
            {
                return BadRequest("Invalid credentials");
            }

            var token = GenerateJwtToken(users);
            return Ok(token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<Member>> CreateUser([FromBody] Member payload)
    {
        try
        {
            payload.Password = _hashpassword.Hpassword(payload.Password);

            await _context.Member.AddAsync(payload);

            await _cartController.CreateCart();

            await _context.SaveChangesAsync();

            return Ok(payload);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string GenerateJwtToken(Member member)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, member.Pseudo),
                new Claim("UserId", member.Id.ToString()),
            };

            if (!string.IsNullOrEmpty(member.Role))
            {
                claims.Add(new Claim("Role", member.Role));
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}