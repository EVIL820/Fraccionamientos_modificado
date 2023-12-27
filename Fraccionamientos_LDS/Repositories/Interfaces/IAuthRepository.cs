using Fraccionamientos_LDS.Entities;

namespace Fraccionamientos_LDS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        User AuthenticateUser(string identifier, string password);
    }
}
