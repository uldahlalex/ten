using System.Security.Cryptography;
using System.Text;

namespace api.Services;

public class CryptographyService : ICryptographyService
{
    public string Hash(string str)
    {
        return Convert.ToBase64String(SHA512.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(str)));
    }
}