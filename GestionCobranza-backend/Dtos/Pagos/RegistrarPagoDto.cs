using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GestionCobranza_backend.Dtos.Pagos;

public class RegistrarPagoDto
{
    [Required]
    public int IdDeuda { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor a cero.")]
    public decimal MontoPagado { get; set; }

    [Required]
    public string MetodoPago { get; set; } = null!; // Ej: EFECTIVO, TRANSFERENCIA, YAPE

    public string? NroOperacion { get; set; }

    public string? Observacion { get; set; }
}