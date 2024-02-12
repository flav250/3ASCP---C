using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct([FromQuery] string sortBy = "Name",
        [FromQuery] int limit = 10)
    {
        try
        {
            var products = await _context.Product.Include(p => p.SellerMember).Select(p => new Product
            {
                Id = p.Id, Name = p.Name, Available = p.Available, Image = p.Image, Price = p.Price,
                AddedTime = p.AddedTime, UserId = p.SellerMember.Id
            }).ToListAsync();
            if (sortBy != "AddedTime" && sortBy != "Name" && sortBy != "Price" && sortBy != "Available")
            {
                return BadRequest("Invalid sortBy parameter");
            }

            var sortedProducts = products.AsQueryable().OrderBy(sortBy).Take(limit).ToList();

            return Ok(sortedProducts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<Product>> GetProductById(int productId)
    {
        try
        {
            var product = await _context.Product.FindAsync(productId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsBySearch([FromQuery] string searchTerm,
        [FromQuery] string sortBy = "Name",
        [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var products = await _context.Product.Include(p => p.SellerMember)
                .Where(p => p.Name.Contains(searchTerm))
                .ToListAsync();

            if (sortBy != "AddedTime" && sortBy != "Name" && sortBy != "Price" && sortBy != "Available")
            {
                return BadRequest("Invalid sortBy parameter");
            }

            var sortedProducts = products.AsQueryable().OrderBy(sortBy).Take(limit).ToList();

            return Ok(sortedProducts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct([FromBody] Product payload)
    {
        try
        {
            var userRole = User.FindFirst("Role")?.Value;
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId) ||
                !int.TryParse(userId, out int userIdToken))
            {
                return BadRequest("Invalid token");
            }

            if (userRole != "seller")
            {
                return Unauthorized("You are not authorized to add products");
            }

            payload.SellerMember = await _context.Member.FindAsync(userIdToken);
            _context.Product.Add(payload);

            await _context.SaveChangesAsync();

            return Ok(payload);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{productId}")]
    public async Task<ActionResult<Product>> PutProduct(int productId, [FromBody] Product payload)
    {
        try
        {
            var userRole = User.FindFirst("Role")?.Value;
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId) ||
                !int.TryParse(userId, out int userIdToken))
            {
                return BadRequest("Invalid token");
            }

            if (userRole != "seller")
            {
                return Unauthorized("You are not authorized to update products");
            }

            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (product.UserId != userIdToken)
            {
                return Unauthorized("You are not authorized to update this product");
            }

            product.Name = payload.Name;
            product.Available = payload.Available;
            product.Image = payload.Image;
            product.Price = payload.Price;
            product.AddedTime = payload.AddedTime;

            await _context.SaveChangesAsync();

            return Ok(product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{productId}")]
    public async Task<ActionResult<Product>> DeleteProduct(int productId)
    {
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    public async Task<ActionResult<IEnumerable<Product>>> DeleteProcductsBySeller(int sellerId)
    {
        try
        {
            var products = await _context.Product.Where(p => p.SellerMember.Id == sellerId).ToListAsync();
            _context.Product.RemoveRange(products);
            await _context.SaveChangesAsync();
            return products;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}