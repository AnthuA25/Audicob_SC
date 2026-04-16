using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public int? IdAsesor { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string EstadoCliente { get; set; } = null!;

    public string Riesgo { get; set; } = null!;

    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<Alertum> Alerta { get; set; } = new List<Alertum>();

    public virtual ICollection<Deudum> Deuda { get; set; } = new List<Deudum>();

    public virtual ICollection<GestionCobranza> GestionCobranzas { get; set; } = new List<GestionCobranza>();

    public virtual Usuario? IdAsesorNavigation { get; set; }
}
