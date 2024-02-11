using System.ComponentModel.DataAnnotations.Schema;

namespace IBayApi2.Models;
[Table("Member")]

public class Member
{
    public int Id { get ; set; }
    public string Email { get ; set; }
    public string Pseudo { get ; set; }
    public string Password { get ; set; }
    public string Role { get ; set; }
}