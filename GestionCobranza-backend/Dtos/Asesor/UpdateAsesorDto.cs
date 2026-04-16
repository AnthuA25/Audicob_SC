namespace GestionCobranza_backend.Dtos.Usuarios;

public class UpdateAsesorDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Password { get; set; }
}