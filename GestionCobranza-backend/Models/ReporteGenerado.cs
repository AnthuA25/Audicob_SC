using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class ReporteGenerado
{
    public int IdReporte { get; set; }

    public int IdUsuario { get; set; }

    public string NombreReporte { get; set; } = null!;

    public string TipoReporte { get; set; } = null!;

    public DateOnly? FechaDesde { get; set; }

    public DateOnly? FechaHasta { get; set; }

    public string? ArchivoUrl { get; set; }

    public DateTime FechaGeneracion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
