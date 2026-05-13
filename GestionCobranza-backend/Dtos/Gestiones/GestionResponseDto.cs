namespace GestionCobranza_backend.Dtos.Gestiones;

public class GestionResponseDto
{
    public int IdGestion { get; set; }
    public int IdCliente { get; set; }
    public int IdUsuario { get; set; }
    public string TipoGestion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaGestion { get; set; }
    public string? Resultado { get; set; }
    public DateOnly? ProximaAccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public bool Activo { get; set; }
    public bool Eliminado { get; set; }
}
