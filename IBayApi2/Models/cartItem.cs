using System.ComponentModel.DataAnnotations.Schema;

namespace IBayApi2.Models;

public class cartItem
{
    public int Id { get; set; }
    [ForeignKey("CartId")] public User CartId { get; set; }
    [ForeignKey("ProductId")] public Product Product { get; set; }
    public int Quantity { get; set; }
}