using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class Deudum
{
    public int IdDeuda { get; set; }

    public int IdCliente { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal SaldoPendiente { get; set; }

    public DateOnly FechaEmision { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public int DiasAtraso { get; set; }

    public string EstadoDeuda { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<Alertum> Alerta { get; set; } = new List<Alertum>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
