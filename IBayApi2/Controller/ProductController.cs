using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Controller;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductController : ControllerBase
{
    private readonly ApiContext _context;
    private readonly Hashpassword _hashpassword;

    public ProductController(ApiContext context, Hashpassword hashpassword)
    {
        _context = context;
        _hashpassword = hashpassword;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        var products = await _context.ProductItems.Include(p => p.SellerUser).Select(p => new Product
        {
            Id = p.Id, Name = p.Name, Available = p.Available, Image = p.Image, Price = p.Price,
            AddedTime = p.AddedTime, UserId = p.SellerUser.Id
        }).ToListAsync();
        return products;
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<Product>> GetProductById(int Id)
    {
        var product = await _context.ProductItems.FindAsync(Id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct([FromBody] Product payload)
    {
        var userRole = User.FindFirst("role")?.Value;

        if (string.IsNullOrEmpty(userRole) || !int.TryParse(userRole, out int userRoleToken))
        {
            return BadRequest();
        }

        if (userRole != "seller")
        {
            return Unauthorized();
        }

        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        payload.SellerUser.Id = userIdToken;
        _context.ProductItems.Add(payload);

        await _context.SaveChangesAsync();

        return Ok(payload);
    }


}