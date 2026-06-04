namespace GestionCobranza_backend.Dtos.Alertas;

public class AlertaResponseDto
{
    public int IdAlerta { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public int? IdDeuda { get; set; }
    public string TipoAlerta { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public bool Leido { get; set; }
    public DateTime FechaAlerta { get; set; }
}
