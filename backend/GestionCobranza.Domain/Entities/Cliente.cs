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

    // HU-04: campos de deuda y vencimiento
    [Column(TypeName = "numeric(12,2)")]
    public decimal deuda_total { get; set; } = 0.00m;

    [Column(TypeName = "numeric(12,2)")]
    public decimal saldo_pendiente { get; set; } = 0.00m;

    public DateOnly? fecha_vencimiento { get; set; }

    public string estado_cliente { get; set; } = "Nuevo";
    public string riesgo { get; set; } = "Bajo";
    public string? observacion { get; set; }

    public DateTime fecha_registro { get; set; } = DateTime.UtcNow;
    public string usuario_registro { get; set; } = null!;

    public DateTime? fecha_modificacion { get; set; }
    public string? usuario_modificacion { get; set; }

    public bool activo { get; set; } = true;
    public bool eliminado { get; set; } = false;
}