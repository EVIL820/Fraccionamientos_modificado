using System;
using System.Text;
using System.Threading.Tasks;
using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Fraccionamientos_LDS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ResidentialContext _context;

        public AuthRepository(ResidentialContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> AuthenticateUserAsync(string identifier, string plainTextPassword, bool isTwoFactorEnabled)
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

                // Hashear la contraseña proporcionada por el usuario antes de verificar
                string hashedPassword = PasswordHasher.HashPassword(plainTextPassword);

                // Verificar si la contraseña proporcionada por el usuario es válida
                if (!PasswordHasher.VerifyPassword(hashedPassword, user.Password))
                {
                    Console.WriteLine("Error: Contraseña incorrecta.");
                    return null;
                }

                // Verificar el estado de la autenticación de doble factor
                if (isTwoFactorEnabled && !user.TwoFactorEnabled)
                {
                    Console.WriteLine("Error: Autenticación de doble factor no activada para este usuario.");
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

        public async Task<bool> ValidateCredentialsAsync(string identifier, string plainTextPassword)
        {
            try
            {
                // Buscar al usuario por nombre de usuario o correo electrónico
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);

                // Verificar si se encontró un usuario
                if (user == null || string.IsNullOrEmpty(user.Password))
                {
                    Console.WriteLine("Error: Usuario o contraseña incorrectos.");
                    throw new Exception("Error en la autenticación: Usuario o contraseña incorrectos.");
                }

                // Hashear la contraseña proporcionada por el usuario antes de verificar
                string hashedPassword = PasswordHasher.HashPassword(plainTextPassword);

                // Verificar si la contraseña proporcionada por el usuario es válida
                if (!PasswordHasher.VerifyPassword(hashedPassword, user.Password))
                {
                    Console.WriteLine("Error: Contraseña incorrecta.");
                    throw new Exception("Error en la autenticación: Contraseña incorrecta.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar credenciales: {ex.ToString()}");
                throw; // Propagar la excepción para obtener más información en la consola
            }
        }

        public async Task<User> GetUserByIdentifierAsync(string identifier)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            try
            {
                byte[] storedHashBytes = Convert.FromBase64String(storedPassword);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputPasswordBytes = Encoding.UTF8.GetBytes(inputPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputPasswordBytes);

                    // Utilizar CompareByteArrays para una comparación segura en términos de tiempo
                    return CompareByteArrays(hashBytes, storedHashBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar la contraseña: {ex.ToString()}");
                throw;
            }
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }
}
