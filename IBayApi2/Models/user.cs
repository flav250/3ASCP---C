namespace IBayApi2.Models;

public class User
{
    public int Id { get ; set; }
    public string email { get ; set; }
    public string pseudo { get ; set; }
    public string password { get ; set; }
    public string role { get ; set; }
}