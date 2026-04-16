namespace GestionCobranza.Application.DTOs;

public class CrearAsesorResponseDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Mensaje { get; set; } = null!;
}
