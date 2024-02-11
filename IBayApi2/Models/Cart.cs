using System.ComponentModel.DataAnnotations.Schema;

namespace IBayApi2.Models;

[Table("Cart")]
public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")] public Member Member { get; set; }
    public bool Buy { get; set; }
}