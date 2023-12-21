using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("La contraseña no puede ser nula o vacía");
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Devolver solo el hash como cadena
                return Convert.ToBase64String(hashBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al hashar la contraseña: {ex.Message}");
            throw;
        }
    }

    public static bool VerifyPassword(string inputPassword, string storedPassword)
    {
        try
        {
            byte[] storedHashBytes = Convert.FromBase64String(storedPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputPasswordBytes = Encoding.UTF8.GetBytes(inputPassword);
                byte[] hashBytes = sha256.ComputeHash(inputPasswordBytes);

                return hashBytes.SequenceEqual(storedHashBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar la contraseña: {ex.Message}");
            throw;
        }
    }
}
