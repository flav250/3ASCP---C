using System.ComponentModel.DataAnnotations.Schema;

namespace IBayApi2.Models;

public class cart
{
    public int Id { get; set; }
    [ForeignKey("UserId")] public User UserId { get; set; }
    public bool Buy { get; set; }
}