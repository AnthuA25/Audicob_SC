namespace GestionCobranza_backend.Dtos.Usuarios;

public class CreateUsuarioDto
{
    public int IdRol { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Password { get; set; } = string.Empty;
}