using System.Security.Cryptography;
using System.Text;

namespace IBayApi2.Data;

public class Hashpassword
{
    public string Hpassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {

            byte[] saltedPassword = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(saltedPassword);

            StringBuilder hashStringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashStringBuilder.Append(b.ToString("x2"));
            }

            return hashStringBuilder.ToString();
        }
    }
}