using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCobranza.Domain.Entities;

[Table("cliente", Schema = "public")]
public class Cliente
{
    [Key]
    public int id_cliente { get; set; }

    public int? id_asesor { get; set; }

    [Required]
    public string nombres { get; set; } = null!;

    [Required]
    public string apellidos { get; set; } = null!;

    [Required]
    public string dni { get; set; } = null!;

    public string? correo { get; set; }
    public string? telefono { get; set; }
    public string? direccion { get; set; }

    public string estado_cliente { get; set; } = "NUEVO";
    public string riesgo { get; set; } = "BAJO";
    public string? observacion { get; set; }

    public DateTime fecha_registro { get; set; } = DateTime.Now;
    public string usuario_registro { get; set; } = null!;

    public DateTime? fecha_modificacion { get; set; }
    public string? usuario_modificacion { get; set; }

    public bool activo { get; set; } = true;
    public bool eliminado { get; set; } = false;
}