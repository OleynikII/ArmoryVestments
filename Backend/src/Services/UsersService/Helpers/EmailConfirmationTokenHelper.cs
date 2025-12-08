namespace UsersService.Helpers;

public interface IEmailTokenHelper
{
    string GenerateTokenByEmail(string email);
}


public class EmailTokenHelper : IEmailTokenHelper
{
    public string GenerateTokenByEmail(string email)
    {
        var emailBytes = Encoding.UTF8.GetBytes(email);
        var randomBytes = new byte[24];
        
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var combinedBytes = new byte[emailBytes.Length + randomBytes.Length];
        Buffer.BlockCopy(emailBytes, 0, combinedBytes, 0, emailBytes.Length);
        Buffer.BlockCopy(randomBytes, 0, combinedBytes, emailBytes.Length, randomBytes.Length);
        
        return Convert.ToBase64String(combinedBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}