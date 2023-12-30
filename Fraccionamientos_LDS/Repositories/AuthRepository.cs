using System;
using System.Linq;
using System.Threading.Tasks;
using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fraccionamientos_LDS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ResidentialContext _context;

        public AuthRepository(ResidentialContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> AuthenticateUserAsync(string identifier, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.UserName == identifier || u.Email == identifier) && u.Password == password);

            return user;
        }

        public async Task<bool> ValidateCredentialsAsync(string identifier, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.UserName == identifier || u.Email == identifier) && u.Password == password);
            return user != null;
        }
    }
}
