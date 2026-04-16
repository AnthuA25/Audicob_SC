using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Models;

public partial class ImportacionDato
{
    public int IdImportacion { get; set; }

    public int IdUsuario { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string? RutaArchivo { get; set; }

    public int TotalRegistros { get; set; }

    public string EstadoImportacion { get; set; } = null!;

    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool Activo { get; set; }

    public bool Eliminado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
