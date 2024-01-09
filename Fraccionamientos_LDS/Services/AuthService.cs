using System.Threading.Tasks;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services.Interfaces;

namespace Fraccionamientos_LDS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAndGetTokenAsync(string identifier, string password, bool isTwoFactorEnabled)
        {
            // Lógica de autenticación y generación de token aquí
            var user = await _authRepository.AuthenticateUserAsync(identifier, password, isTwoFactorEnabled);

            // Aquí utilizamos el servicio JwtService para generar el token.
            var token = _jwtService.GenerateJwtToken(user);

            return token;
        }

        public async Task<bool> ActivateTwoFactorAsync(string userId)
        {
            return await _authRepository.ActivateTwoFactorAsync(userId);
        }

        public async Task<bool> DeactivateTwoFactorAsync(string userId)
        {
            return await _authRepository.DeactivateTwoFactorAsync(userId);
        }
    }
}
