using System.Security.Cryptography;

namespace UserService.Api.Helpers;

public interface IResetPasswordCodeHelper
{
    string GenerateCode();
}

public class ResetPasswordCodeHelper : IResetPasswordCodeHelper
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    
    public string GenerateCode()
    {
        var randomBytes = new byte[4];
        _rng.GetBytes(randomBytes);
        
        var randomNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 100_000_000;
        
        return randomNumber.ToString("D8");
    }
}