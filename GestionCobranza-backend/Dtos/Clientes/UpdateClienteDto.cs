namespace GestionCobranza_backend.Dtos.Clientes;

public class UpdateClienteDto
{
    public int? IdAsesor { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string EstadoCliente { get; set; } = string.Empty;
    public string Riesgo { get; set; } = string.Empty;
    public decimal? MontoDeuda { get; set; }

    public DateOnly? FechaVencimiento { get; set; }
    
    public string? Observacion { get; set; }
}