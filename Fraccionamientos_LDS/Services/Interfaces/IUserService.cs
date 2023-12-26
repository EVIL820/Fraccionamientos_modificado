using Fraccionamientos_LDS.Entities;
using System.Collections.Generic;

namespace Fraccionamientos_LDS.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(int userId, User updatedUser);
        void DeleteUser(int id);
        User AuthenticateUser(string userNameOrEmail, string password);
    }
}
   