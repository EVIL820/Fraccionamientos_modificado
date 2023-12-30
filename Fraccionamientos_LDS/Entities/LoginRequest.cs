namespace Fraccionamientos_LDS.Entities
{
    public class LoginRequest
    {
        public string Identifier { get; set; } // Puede ser el correo electrónico o el nombre de usuario
        public string Password { get; set; }
    }
}
