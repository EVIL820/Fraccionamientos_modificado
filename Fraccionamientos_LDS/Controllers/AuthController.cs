using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(IUserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            if (string.IsNullOrEmpty(loginRequest?.Identifier) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Debe proporcionar un identificador y una contraseña");
            }

            // Verificar credenciales utilizando el servicio de usuario
            var user = _userService.AuthenticateUser(loginRequest.Identifier, loginRequest.Password);

            if (user == null)
            {
                // Credenciales inválidas
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            // Generar token JWT (aquí puedes incluir la lógica para generar el token según tus necesidades)
            var token = _jwtService.GenerateJwtToken(user.UserId.ToString(), user.UserName);

            // Retornar el token en la respuesta
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción durante el proceso de autenticación
            return StatusCode(500, new { message = $"Error en la autenticación: {ex.Message}" });
        }
    }
}
