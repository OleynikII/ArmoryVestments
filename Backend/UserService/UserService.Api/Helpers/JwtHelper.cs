using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Shared.Claims;

namespace UserService.Api.Helpers;

public interface IJwtHelper
{
    string GenerateRefreshToken();
    string GenerateJwtToken(User user);
}

public class JwtHelper : IJwtHelper
{
    private readonly JwtOptions _jwtOptions;

    public JwtHelper(
        IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateJwtToken(User user) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user));

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(Convert.ToInt32(_jwtOptions.ExpirationSeconds)),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        var rolesEntities = user.Roles;

        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var roleEntity in rolesEntities)
        {
            roleClaims.Add(new Claim(ApplicationClaimTypes.Role, roleEntity.Title));
            permissionClaims.AddRange(roleEntity.Permissions
                .Select(permissionEntity =>
                    new Claim(ApplicationClaimTypes.Permission, permissionEntity.Title)));
        }

        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.UserId, user.Id.ToString()),
            new(ApplicationClaimTypes.Email, user.Email),
            new(ApplicationClaimTypes.UserName, user.UserName)
        }.Union(roleClaims).Union(permissionClaims);

        return claims;
    }

    private SigningCredentials GetSigningCredentials() =>
        new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256);
}