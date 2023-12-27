using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IAuthRepository authRepository, ILogger<UserService> logger, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _logger = logger;
        _configuration = configuration;
    }

    [SwaggerOperation(Summary = "Obtiene todos los usuarios.")]
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    [SwaggerOperation(Summary = "Obtiene un usuario por su Id.")]
    public User GetUserById(int id)
    {
        return _userRepository.GetUserById(id);
    }

    [SwaggerOperation(Summary = "Crea un nuevo usuario.")]
    public void CreateUser(User user)
    {
        try
        {
            if (user == null || !IsValidStringValue(user.UserName) || !IsValidStringValue(user.Email) || !IsValidStringValue(user.Password))
            {
                throw new ArgumentException("Los campos UserName, Email y Password no pueden ser nulos ni iguales a 'string'.");
            }

            // Obtener el valor de la configuración
            bool encryptPasswords = _configuration.GetValue<bool>("AppSettings:EncryptPasswords");

            if (encryptPasswords)
            {
                // Verificar si la contraseña cumple con los requisitos
                if (!PasswordHasher.IsPasswordStrong(user.Password))
                {
                    // Mensaje indicando los requisitos de la contraseña
                    throw new ArgumentException("La contraseña debe contener al menos una mayúscula, un número y un carácter especial.");
                }

                // Hash de la nueva contraseña antes de almacenarla
                user.Password = PasswordHasher.HashPassword(user.Password);
            }

            _userRepository.CreateUser(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear un nuevo usuario: {ex.Message}");
            throw;
        }
    }

    [SwaggerOperation(Summary = "Actualiza un usuario.")]
    public void UpdateUser(int userId, User updatedUser)
    {
        try
        {
            var existingUser = _userRepository.GetUserById(userId);

            if (existingUser == null)
            {
                _logger.LogError($"Usuario con ID {userId} no encontrado");
                throw new ArgumentException($"Usuario con ID {userId} no encontrado");
            }

            // Actualizar solo los campos válidos
            existingUser.UserName = IsValidUpdateValue(updatedUser.UserName, existingUser.UserName) ? updatedUser.UserName : existingUser.UserName;
            existingUser.Email = IsValidUpdateValue(updatedUser.Email, existingUser.Email) ? updatedUser.Email : existingUser.Email;

            // Verificar y actualizar la contraseña si se proporciona
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                // Verificar que la nueva contraseña sea fuerte
                if (!PasswordHasher.IsPasswordStrong(updatedUser.Password))
                {
                    // Mensaje indicando los requisitos de la contraseña
                    throw new ArgumentException("La contraseña debe contener al menos una mayúscula, un número y un carácter especial.");
                }

                // Hash de la nueva contraseña antes de almacenarla
                existingUser.Password = PasswordHasher.HashPassword(updatedUser.Password);
            }

            _userRepository.UpdateUser(existingUser);

            _logger.LogInformation($"Usuario con ID {userId} actualizado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar un usuario: {ex.Message}");
            throw;
        }
    }

    [SwaggerOperation(Summary = "Elimina un usuario por su Id.")]
    public void DeleteUser(int id)
    {
        try
        {
            _userRepository.DeleteUser(id);
            _logger.LogInformation($"Usuario con ID {id} eliminado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar un usuario: {ex.Message}");
            throw;
        }
    }

    private bool IsValidStringValue(string value)
    {
        return !string.IsNullOrEmpty(value) && !value.ToLower().Equals("string");
    }

    private bool IsValidUpdateValue(string updatedValue, string currentValue)
    {
        return !string.IsNullOrEmpty(updatedValue) && !updatedValue.ToLower().Equals("string", StringComparison.OrdinalIgnoreCase) && !updatedValue.Equals(currentValue);
    }
}
