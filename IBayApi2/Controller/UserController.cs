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
        try
        {
            var users = await _context.Member.Select(u => new Member
                { Id = u.Id, Pseudo = u.Pseudo, Email = u.Email, Role = u.Role }).ToListAsync();
            return Ok(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Member>> GetUserById(int userId)
    {
        try
        {
            var user = await _context.Member.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<ActionResult<Member>> PutUser([FromBody] Member payload)
    {
        try
        {
            var userId = User.FindFirst("UserId")?.Value;

            var hashpassword = _hashpassword.Hpassword(payload.Password);

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
            {
                return BadRequest("Invalid token");
            }

            var user = await _context.Member.FirstOrDefaultAsync(u => u.Id == userIdToken);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Email = payload.Email;
            user.Pseudo = payload.Pseudo;
            user.Password = hashpassword;
            user.Role = payload.Role;

            await _context.SaveChangesAsync();

            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    public async Task<ActionResult<Member>> DeleteUser()
    {
        try
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
            {
                return BadRequest("Invalid token");
            }

            var user = await _context.Member.FindAsync(userIdToken);

            if (user == null)
            {
                return NotFound("User not found");
            }

            await _productController.DeleteProductsBySeller(userIdToken);

            var cartToRemove = await _context.Cart.Where(c => c.Member.Id == userIdToken).FirstOrDefaultAsync();
            if (cartToRemove != null)
            {
                _context.Cart.Remove(cartToRemove);
            }
            var cartItemsToRemove = await _context.CartItem.Where(ci => ci.Cart.Member.Id == userIdToken).ToListAsync();
            if (cartItemsToRemove != null)
            {
                _context.CartItem.RemoveRange(cartItemsToRemove);
            }

            _context.Member.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}