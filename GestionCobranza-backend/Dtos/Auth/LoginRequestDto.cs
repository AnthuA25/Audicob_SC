namespace GestionCobranza_backend.Dtos.Auth;

public class LoginRequestDto
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}