namespace GestionCobranza_backend.Dtos.Roles;

public class CreateRolDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}