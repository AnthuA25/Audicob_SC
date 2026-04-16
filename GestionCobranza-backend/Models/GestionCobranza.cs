using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class GestionCobranza
{
    public int IdGestion { get; set; }

    public int IdCliente { get; set; }

    public int IdUsuario { get; set; }

    public string TipoGestion { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaGestion { get; set; }

    public string? Resultado { get; set; }

    public DateOnly? ProximaAccion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
