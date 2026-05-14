namespace GestionCobranza_backend.Dtos.Pagos;

public class PagoResponseDto
{
    public int IdPago { get; set; }
    public int IdDeuda { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateOnly FechaPago { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string? Nota { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
}
