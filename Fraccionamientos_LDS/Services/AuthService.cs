using System.Threading.Tasks;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services.Interfaces;

namespace Fraccionamientos_LDS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;  // Asumo que tienes una interfaz para tu servicio Jwt, llamémosla IJwtService.

        public AuthService(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAndGetTokenAsync(string identifier, string password)
        {
            // Lógica de autenticación y generación de token aquí
            var user = await _authRepository.AuthenticateUserAsync(identifier, password);

            // Aquí utilizamos el servicio JwtService para generar el token.
            var token = _jwtService.GenerateJwtToken(user);

            return token;
        }
    }
}
