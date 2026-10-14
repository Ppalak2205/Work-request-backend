using Work_request.Models;

namespace Work_request.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
}
