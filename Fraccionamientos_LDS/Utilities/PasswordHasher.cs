using System;
using System.Linq;
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

            // Agrega requisitos para la contraseña (mayúsculas, números, caracteres especiales)
            if (!IsPasswordStrong(password))
            {
                throw new ArgumentException("La contraseña debe contener al menos una mayúscula, un número y un carácter especial.");
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

                // Utilizar CompareByteArrays para una comparación segura en términos de tiempo
                return CompareByteArrays(hashBytes, storedHashBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al verificar la contraseña: {ex.Message}");
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

    public static bool IsPasswordStrong(string password)
    {
        // Requiere al menos una mayúscula, un número y un carácter especial
        return password.Any(char.IsUpper) && password.Any(char.IsDigit) && password.Any(IsSpecialCharacter);
    }

    private static bool IsSpecialCharacter(char c)
    {
        // Puedes personalizar esta lógica según los caracteres especiales que deseas permitir
        return !char.IsLetterOrDigit(c);
    }
}
