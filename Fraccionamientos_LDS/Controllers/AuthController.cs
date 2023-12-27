using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtService _jwtService;

    public AuthController(IAuthRepository authRepository, JwtService jwtService)
    {
        _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        Console.WriteLine($"Entrando en AuthController/Login: identifier={loginRequest.Identifier}");

        try
        {
            if (string.IsNullOrEmpty(loginRequest?.Identifier) || string.IsNullOrEmpty(loginRequest.Password))
            {
                Console.WriteLine("Datos de autenticación no válidos");
                return BadRequest("Debe proporcionar un identificador y una contraseña");
            }

            var user = _authRepository.AuthenticateUser(loginRequest.Identifier, loginRequest.Password);

            if (user == null)
            {
                Console.WriteLine("Credenciales inválidas");
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            Console.WriteLine($"Autenticación exitosa para usuario: {user.UserName}");

            var token = _jwtService.GenerateJwtToken(user.UserId.ToString(), user.UserName);

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la autenticación: {ex.Message}");
            return StatusCode(500, new { message = $"Error en la autenticación: {ex.Message}" });
        }
    }
}
