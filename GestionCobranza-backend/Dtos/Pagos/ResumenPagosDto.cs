namespace GestionCobranza_backend.Dtos.Pagos;

public class ResumenPagosDto
{
    public decimal TotalPagosHoy { get; set; }
    public int TransaccionesHoy { get; set; }
    public decimal TotalPagosSemana { get; set; }
    public int TransaccionesSemana { get; set; }
    public decimal TotalPagosMes { get; set; }
    public int TransaccionesMes { get; set; }
}
