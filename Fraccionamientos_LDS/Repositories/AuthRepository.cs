using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using System;

namespace Fraccionamientos_LDS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ResidentialContext _context;

        public AuthRepository(ResidentialContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User AuthenticateUser(string identifier, string password)
        {
            // Implementa la lógica de autenticación aquí
            // Puedes utilizar el UserRepository o cualquier otra lógica que necesites

            // Ejemplo:
            var user = _context.Users.FirstOrDefault(u => (u.UserName == identifier || u.Email == identifier) && u.Password != null);

            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                return user;
            }

            return null;
        }
    }
}
