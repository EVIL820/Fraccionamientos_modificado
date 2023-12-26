using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services;
using Swashbuckle.AspNetCore.Annotations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
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
        if (user != null)
        {
            // Validar que UserName, Email y Password no sean nulos ni iguales a "string"
            if (IsValidStringValue(user.UserName) && IsValidStringValue(user.Email) && IsValidStringValue(user.Password))
            {
                // Hash de la nueva contraseña antes de almacenarla
                user.Password = PasswordHasher.HashPassword(user.Password);
                _userRepository.CreateUser(user);
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
        var existingUser = _userRepository.GetUserById(userId);

        if (existingUser == null)
        {
            _logger.LogError($"Usuario con ID {userId} no encontrado");
            throw new ArgumentException($"Usuario con ID {userId} no encontrado");
        }
        
        // Actualizar solo los campos válidos y diferentes de "string"
        existingUser.UserName = IsValidUpdateValue(updatedUser.UserName, existingUser.UserName) ? updatedUser.UserName : existingUser.UserName;
        existingUser.Email = IsValidUpdateValue(updatedUser.Email, existingUser.Email) ? updatedUser.Email : existingUser.Email;
        existingUser.Password = IsValidUpdateValue(updatedUser.Password, existingUser.Password) ? PasswordHasher.HashPassword(updatedUser.Password) : existingUser.Password;

        _userRepository.UpdateUser(existingUser);

        _logger.LogInformation($"Usuario con ID {userId} actualizado correctamente");
        }

    [SwaggerOperation(Summary = "Elimina un usuario por su Id.")]
    public void DeleteUser(int id)
    {
        _userRepository.DeleteUser(id);
        _logger.LogInformation($"Usuario con ID {id} eliminado correctamente");
    }

    [SwaggerOperation(Summary = "Autentica un usuario.")]
    public User AuthenticateUser(string identifier, string password)
    {
        Console.WriteLine($"Entrando en AuthenticateUser: identifier={identifier}");

        // Validar que los valores no sean nulos ni iguales a "string"
        if (!string.IsNullOrEmpty(identifier) && !string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Valores de entrada son válidos");

            // Obtener el usuario por nombre de usuario o correo electrónico
            var user = _userRepository.GetUserByUserNameOrEmail(identifier);

            Console.WriteLine($"Usuario recuperado: {user?.UserName}");

            // Verificar si el usuario existe y comparar contraseñas
            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                Console.WriteLine("Contraseña verificada - Autenticación exitosa");
                return user;
            }
            else
            {
                Console.WriteLine("Contraseña no verificada - Autenticación fallida");
            }
        }

        Console.WriteLine("Valores de entrada no válidos - Autenticación fallida");
        // Devolver null si los valores no son válidos o la autenticación falla
        return null;
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
