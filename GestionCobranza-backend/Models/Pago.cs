using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdDeuda { get; set; }

    public decimal Monto { get; set; }

    public DateOnly FechaPago { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? ComprobanteUrl { get; set; }

    public string? Nota { get; set; }

    public string EstadoPago { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual Deudum IdDeudaNavigation { get; set; } = null!;
}
