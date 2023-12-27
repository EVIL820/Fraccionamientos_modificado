using Fraccionamientos_LDS.Data;

namespace Fraccionamientos_LDS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        User AuthenticateUser(string identifier, string password);
        Task<User> AuthenticateUserAsync(string identifier, string password);
        ResidentialContext GetContext();  // Agregado el nuevo método
    }
}
