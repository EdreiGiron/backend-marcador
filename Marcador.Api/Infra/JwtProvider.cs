using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marcador.Api.Infra;

public class JwtProvider
{
    private readonly IConfiguration _config;

    public JwtProvider(IConfiguration config)
    {
        _config = config;
    }

public string GenerateToken(string username, string email, string? role = null)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Email, email)
    };

    if (!string.IsNullOrEmpty(role))
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!)
    );
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var expiryMinutes = int.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "60");

    var token = new JwtSecurityToken(
        issuer: _config["JwtSettings:Issuer"],
        audience: _config["JwtSettings:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}
