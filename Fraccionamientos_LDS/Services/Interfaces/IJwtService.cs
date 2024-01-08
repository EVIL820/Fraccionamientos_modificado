using System.Security.Claims;

namespace Fraccionamientos_LDS.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
        ClaimsPrincipal ValidateJwtToken(string token);
    }
}
