using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class Alertum
{
    public int IdAlerta { get; set; }

    public int? IdCliente { get; set; }

    public int? IdDeuda { get; set; }

    public string TipoAlerta { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string Prioridad { get; set; } = null!;

    public bool Leido { get; set; }

    public DateTime FechaAlerta { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Deudum? IdDeudaNavigation { get; set; }
}
