namespace GestionCobranza.Application.DTOs;

public class AsesorDetalleDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}
