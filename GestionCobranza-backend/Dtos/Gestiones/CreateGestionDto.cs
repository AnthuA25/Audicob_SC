namespace GestionCobranza_backend.Dtos.Gestiones;

public class CreateGestionDto
{
    public int IdCliente { get; set; }
    public string TipoGestion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Resultado { get; set; }
    public DateOnly? ProximaAccion { get; set; }
}
