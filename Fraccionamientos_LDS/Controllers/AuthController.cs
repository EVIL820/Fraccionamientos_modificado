using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Inicia sesión y obtiene un token JWT.")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Identifier) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Debe proporcionar un identificador y una contraseña válidos.");
            }

            // Validación mejorada de las credenciales antes de intentar la autenticación.
            var isValidCredentials = await _authRepository.ValidateCredentialsAsync(loginRequest.Identifier, loginRequest.Password);

            if (!isValidCredentials)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var user = await _authRepository.AuthenticateUserAsync(loginRequest.Identifier, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // Autenticación exitosa
            var token = _jwtService.GenerateJwtToken(user);

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            // Manejo adecuado de errores y respuesta al cliente.
            return StatusCode(500, new { message = $"Error en la autenticación: {ex.Message}" });
        }
    }
}
