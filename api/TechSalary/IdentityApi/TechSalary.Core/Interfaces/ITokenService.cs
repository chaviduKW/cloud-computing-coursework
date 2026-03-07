using TechSalaryIdentity.Core.Entities;

namespace TechSalaryIdentity.Core.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    AuthRefreshToken GenerateRefreshToken(Guid userId);
    bool ValidateRefreshToken(string token);
}
