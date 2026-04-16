namespace GestionCobranza_backend.Dtos.Auth;

public class LoginResponseDto
{
    public int IdUsuario { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}