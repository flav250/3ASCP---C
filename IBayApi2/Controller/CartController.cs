using IBayApi2.Data;
using IBayApi2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Controller;

[Route("[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ApiContext _context;

    public CartController(ApiContext context)
    {
        _context = context;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItem()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var cart = await _context.Cart.Where(c => c.Member.Id == userIdToken).FirstOrDefaultAsync();
        if (cart == null)
        {
            return NotFound("No cart found");
        }

        Console.WriteLine("before");
        var cartProducts =
            await _context.CartItem.Where(c => c.Cart.Id == cart.Id).Include(c => c.Product).ToListAsync();
        Console.WriteLine("cartProducts" + cartProducts);
        if (cartProducts.Count == 0)
        {
            return NotFound("No items in cart");
        }

        return cartProducts;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult<CartItem>> PostCartItem([FromBody] CartItem payload)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var cart = await _context.Cart.Where(c => c.Member.Id == userIdToken).Include(c => c.Member)
            .FirstOrDefaultAsync();
        if (cart == null)
        {
            return NotFound("No cart found");
        }

        payload.CartId = cart.Id;

        await _context.CartItem.AddAsync(payload);
        await _context.SaveChangesAsync();

        return payload;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{cartItemId}")]
    public async Task<ActionResult<CartItem>> DeleteCartItem(int cartItemId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var cart = await _context.Cart.Where(c => c.Member.Id == userIdToken).FirstOrDefaultAsync();
        if (cart == null)
        {
            return NotFound("No cart found");
        }

        var cartItem = await _context.CartItem.FindAsync(cartItemId);
        if (cartItem == null)
        {
            return NotFound("No item found");
        }

        if (cartItem.Cart.Id != cart.Id)
        {
            return BadRequest("Item not in cart");
        }

        _context.CartItem.Remove(cartItem);
        await _context.SaveChangesAsync();

        return cartItem;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<ActionResult<Cart>> PutCart([FromBody] Cart payload)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdToken))
        {
            return BadRequest();
        }

        var cart = await _context.Cart.Where(c => c.UserId == userIdToken).FirstOrDefaultAsync();
        if (cart == null)
        {
            return NotFound("No cart found");
        }

        cart.Buy = payload.Buy;

        var cartItemsToRemove = await _context.CartItem.Where(c => c.Cart.UserId == userIdToken).ToListAsync();
        _context.CartItem.RemoveRange(cartItemsToRemove);

        await _context.SaveChangesAsync();

        return cart;
    }
}