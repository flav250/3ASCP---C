using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IBayApi2.Controller;


[Route("[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly ApiContext _context;
    private readonly Hashpassword _hashpassword;
    private readonly ProductController _productController;

    public UserController(ApiContext context, Hashpassword hashpassword, ProductController productController)
    {
        _context = context;
        _hashpassword = hashpassword;
        _productController = productController;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Member>>> GetUser()
    {
        var users = await _context.Member.Select(u => new Member
            { Id = u.Id, Pseudo = u.Pseudo, Email = u.Email, Role = u.Role }).ToListAsync();
        return users;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Member>> GetUserById(int userId)
    {
        var user = await _context.Member.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<ActionResult<Member>> PutUser([FromBody] Member payload)
    {
        var userId = User.FindFirst("UserId")?.Value;

        var hashpassword = _hashpassword.Hpassword(payload.Password);

        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var user = await _context.Member.FirstOrDefaultAsync(u => u.Id == userIdToken);

        if (user == null)
        {
            return NotFound();
        }

        user.Email = payload.Email;
        user.Pseudo = payload.Pseudo;
        user.Password = hashpassword;
        user.Role = payload.Role;

        await _context.SaveChangesAsync();

        return user;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    public async Task<ActionResult<Member>> DeleteUser()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var user = await _context.Member.FindAsync(userIdToken);

        if (user == null)
        {
            return NotFound();
        }

        await _productController.DeleteProcductsBySeller(userIdToken);
        _context.Member.Remove(user);

        await _context.SaveChangesAsync();

        return user;
    }
}