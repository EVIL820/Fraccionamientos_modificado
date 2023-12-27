namespace Fraccionamientos_LDS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAndGetTokenAsync(string identifier, string password);
    }
}