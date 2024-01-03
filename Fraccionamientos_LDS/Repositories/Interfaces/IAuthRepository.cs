using System.Threading.Tasks;

namespace Fraccionamientos_LDS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> ValidateCredentialsAsync(string identifier, string password);
        Task<User> AuthenticateUserAsync(string identifier, string password);
        Task<User> GetUserByIdentifierAsync(string identifier);
    }
}
