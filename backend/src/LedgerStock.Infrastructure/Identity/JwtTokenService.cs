using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LedgerStock.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LedgerStock.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAt) GenerateToken(
        string userId,
        string email,
        string fullName,
        IEnumerable<string> roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured.");
        var expiresInMinutes = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "120");

        var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new("fullName", fullName),
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}