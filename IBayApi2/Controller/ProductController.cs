using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Controller;

[Route("[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ApiContext _context;

    public ProductController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
    {
        var products = await _context.Product.Include(p => p.SellerUser).Select(p => new Product
        {
            Id = p.Id, Name = p.Name, Available = p.Available, Image = p.Image, Price = p.Price,
            AddedTime = p.AddedTime, UserId = p.SellerUser.Id
        }).ToListAsync();
        return products;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<Product>> GetProductById(int productId)
    {
        var product = await _context.Product.FindAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct([FromBody] Product payload)
    {
        var userRole = User.FindFirst("Role")?.Value;
        Console.WriteLine("role" + userRole);
        if (string.IsNullOrEmpty(userRole))
        {
            return BadRequest();
        }

        if (userRole != "seller")
        {
            return Unauthorized();
        }

        var userId = User.FindFirst("UserId")?.Value;
        Console.WriteLine("user" + userId);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        payload.SellerUser = await _context.Users.FindAsync(userIdToken);
        _context.Product.Add(payload);

        await _context.SaveChangesAsync();

        return Ok(payload);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{productId}")]
    public async Task<ActionResult<Product>> PutProduct(int productId, [FromBody] Product payload)
    {
        var userRole = User.FindFirst("Role")?.Value;
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId) ||
            !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        if (userRole != "seller")
        {
            return Unauthorized();
        }

        var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        if (product.UserId != userIdToken)
        {
            return Unauthorized();
        }

        product.Name = payload.Name;
        product.Available = payload.Available;
        product.Image = payload.Image;
        product.Price = payload.Price;
        product.AddedTime = payload.AddedTime;

        await _context.SaveChangesAsync();

        return product;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{productId}")]
    public async Task<ActionResult<Product>> DeleteProduct(int productId)
    {
        var userRole = User.FindFirst("Role")?.Value;
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId) ||
            !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        if (userRole != "seller")
        {
            return Unauthorized();
        }

        var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        if (product.UserId != userIdToken)
        {
            return Unauthorized();
        }

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        return product;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    public async Task<ActionResult<IEnumerable<Product>>> DeleteProcductsBySeller(int sellerId)
    {
        var products = await _context.Product.Where(p => p.SellerUser.Id == sellerId).ToListAsync();
        _context.Product.RemoveRange(products);
        await _context.SaveChangesAsync();
        return products;
    }
}