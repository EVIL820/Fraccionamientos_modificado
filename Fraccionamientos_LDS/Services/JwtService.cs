using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fraccionamientos_LDS.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly bool _encryptPasswords;

        public JwtService(IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            if (jwtSettings?.Value == null)
            {
                throw new InvalidOperationException("JwtSettings is null or not properly configured.");
            }

            _jwtSettings = jwtSettings.Value;
            _encryptPasswords = configuration.GetValue<bool>("AppSettings:EncryptPasswords");
        }

        public string GenerateJwtToken(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "El objeto User no puede ser nulo.");
                }

                if (string.IsNullOrEmpty(user.UserName))
                {
                    throw new InvalidOperationException("El nombre de usuario no puede ser nulo o vacío al generar el token JWT.");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                if (tokenString == null)
                {
                    throw new InvalidOperationException("No se pudo generar el token JWT correctamente.");
                }

                return tokenString;
            }
            catch (Exception ex)
            {
                // Loguear la excepción para diagnóstico
                Console.WriteLine($"Error en la generación del token JWT: {ex.Message}");
                throw; // Propagar la excepción para que sea manejada por el código superior
            }
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                SecurityToken validatedToken;

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error al validar el token JWT: {ex.Message}");
                throw;
            }
        }
    }
}
