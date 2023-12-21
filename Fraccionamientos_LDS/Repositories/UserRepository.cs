using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Fraccionamientos_LDS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ResidentialContext _context;

        public UserRepository(ResidentialContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public User GetUserByUserNameOrEmail(string userNameOrEmail)
        {
            Console.WriteLine($"Entrando en GetUserByUserNameOrEmail: userNameOrEmail={userNameOrEmail}");

            var user = _context.Users.FirstOrDefault(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);

            Console.WriteLine($"Usuario recuperado: {user?.UserName}");

            return user;
        }

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
