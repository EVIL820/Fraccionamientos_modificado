using System;
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
            try
            {
                // Buscar al usuario por nombre de usuario o correo electrónico
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);

                // Verificar si se encontró un usuario
                if (user == null || string.IsNullOrEmpty(user.Password))
                {
                    Console.WriteLine("Error: Usuario o contraseña incorrectos.");
                    return null;
                }

                // Verificar si la contraseña es válida
                if (!PasswordHasher.VerifyPassword(password, user.Password))
                {
                    Console.WriteLine("Error: Contraseña incorrecta.");
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la autenticación: {ex.ToString()}");
                throw; // Propagar la excepción para obtener más información en la consola
            }

        }

        public async Task<bool> ValidateCredentialsAsync(string identifier, string password)
        {
            try
            {
                // Buscar al usuario por nombre de usuario o correo electrónico
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);

                // Verificar si se encontró un usuario y si la contraseña es válida
                return user != null && PasswordHasher.VerifyPassword(password, user.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar credenciales: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUserByIdentifierAsync(string identifier)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);
        }
    }
}
