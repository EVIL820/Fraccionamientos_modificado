<<<<<<< HEAD
﻿using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
=======
﻿// UserService.cs
using Fraccionamientos_LDS.Data;
using Swashbuckle.AspNetCore.Annotations;
>>>>>>> 95289807fda448f5127437ac7993301f369a2040

public class UserService : IUserService
{
<<<<<<< HEAD
    private readonly ResidentialContext _context;

    public UserService(ResidentialContext context)
=======
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(int userId, User updatedUser);
        void DeleteUser(int id);
        User AuthenticateUser(string userName, string password);
    }

    public class UserService : IUserService
>>>>>>> 95289807fda448f5127437ac7993301f369a2040
    {
        _context = context;
    }

    [SwaggerOperation(Summary = "Obtiene todos los usuarios.")]
    public IEnumerable<User> GetUsers()
    {
        return _context.Users.ToList();
    }

    [SwaggerOperation(Summary = "Obtiene un usuario por su Id.")]
    public User GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.UserId == id);
    }

    [SwaggerOperation(Summary = "Crea un nuevo usuario.")]
    public void CreateUser(User user)
    {
        if (user != null)
        {
            // Validar que UserName, Email y Password no sean nulos ni iguales a "string"
            if (IsValidStringValue(user.UserName) && IsValidStringValue(user.Email) && IsValidStringValue(user.Password))
            {
                // Hash de la nueva contraseña antes de almacenarla
                user.Password = PasswordHasher.HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else
            {
                // Devolver un código de estado 400 Bad Request con un mensaje de error detallado
                throw new ArgumentException("Los campos UserName, Email y Password no pueden ser nulos ni iguales a 'string'.");
            }
        }
    }

    [SwaggerOperation(Summary = "Actualiza un usuario.")]
    public void UpdateUser(int userId, User updatedUser)
    {
        var existingUser = _context.Users.Find(userId);

        if (existingUser != null && updatedUser != null)
        {
            bool isAnyFieldUpdated = false;

            // Actualizar UserName si se proporciona y es diferente de "string"
            if (IsValidUpdateValue(updatedUser.UserName, existingUser.UserName))
            {
                existingUser.UserName = updatedUser.UserName;
                isAnyFieldUpdated = true;
            }

            // Actualizar Email si se proporciona y es diferente de "string"
            if (IsValidUpdateValue(updatedUser.Email, existingUser.Email))
            {
                existingUser.Email = updatedUser.Email;
                isAnyFieldUpdated = true;
            }

            // Actualizar Password si se proporciona y es diferente de "string"
            if (IsValidUpdateValue(updatedUser.Password))
            {
                // Hash de la nueva contraseña antes de almacenarla
                existingUser.Password = PasswordHasher.HashPassword(updatedUser.Password);
                isAnyFieldUpdated = true;
            }

            if (isAnyFieldUpdated)
            {
                _context.SaveChanges();
            }
            else
            {
                // Devolver un código de estado 400 Bad Request con un mensaje de error detallado
                throw new ArgumentException("Al menos un campo (UserName, Email, Password) debe ser diferente de 'string' para realizar la actualización.");
            }
        }
    }

    private bool IsValidUpdateValue(string updatedValue, string currentValue)
    {
        return updatedValue != null && !string.Equals(updatedValue, "string", StringComparison.OrdinalIgnoreCase) && !string.Equals(updatedValue, currentValue);
    }

    private bool IsValidUpdateValue(string updatedValue)
    {
        return updatedValue != null && !string.Equals(updatedValue, "string", StringComparison.OrdinalIgnoreCase);
    }

    [SwaggerOperation(Summary = "Elimina un usuario por su Id.")]
    public void DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    [SwaggerOperation(Summary = "Autentica un usuario.")]
    public User AuthenticateUser(string userName, string password)
    {
        // Validar que los valores no sean nulos ni iguales a "string"
        if (IsValidStringValue(userName) && IsValidStringValue(password))
        {
            // Hash de la contraseña antes de buscar en la base de datos
            string hashedPassword = PasswordHasher.HashPassword(password);

            // Obtener el usuario con el nombre de usuario proporcionado y la contraseña hasheada
            return _context.Users.FirstOrDefault(u => u.UserName == userName && u.Password == hashedPassword);
        }

        // Devolver null si los valores no son válidos
        return null;
    }

    private bool IsValidStringValue(string value)
    {
        return !string.IsNullOrEmpty(value) && value.ToLower() != "string";
    }
}
