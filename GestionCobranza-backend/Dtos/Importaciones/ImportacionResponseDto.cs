namespace GestionCobranza_backend.Dtos.Importaciones;

public class ImportacionResponseDto
{
    public int IdImportacion { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string? RutaArchivo { get; set; }
    public int TotalRegistros { get; set; }
    public string EstadoImportacion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}