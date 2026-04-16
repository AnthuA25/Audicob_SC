using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<GestionCobranza> GestionCobranzas { get; set; } = new List<GestionCobranza>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<ImportacionDato> ImportacionDatos { get; set; } = new List<ImportacionDato>();

    public virtual ICollection<ReporteGenerado> ReporteGenerados { get; set; } = new List<ReporteGenerado>();
}
