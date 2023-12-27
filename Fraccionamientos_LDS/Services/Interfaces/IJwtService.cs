namespace Fraccionamientos_LDS.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}
