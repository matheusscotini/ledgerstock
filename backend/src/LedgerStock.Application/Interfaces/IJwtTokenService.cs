namespace LedgerStock.Application.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) GenerateToken(
        string userId,
        string email,
        string fullName,
        IEnumerable<string> roles
    );
}