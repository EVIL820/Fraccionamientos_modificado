using Fraccionamientos_LDS.Entities;

namespace Fraccionamientos_LDS.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
    }
}
