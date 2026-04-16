using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCobranza.Domain.Entities;

[Table("pago", Schema = "public")]
public class Pago
{
    [Key]
    public int id { get; set; }

    public int id_cliente { get; set; }
    public int id_asesor { get; set; }

    [Column(TypeName = "numeric(12,2)")]
    public decimal monto { get; set; }

    public DateOnly fecha_pago { get; set; }

    public string metodo_pago { get; set; } = null!;

    // 'Pendiente' | 'Confirmado' | 'Rechazado'
    public string estado_pago { get; set; } = "Confirmado";

    public string? notas { get; set; }

    public DateTime fecha_registro { get; set; } = DateTime.UtcNow;
    public string? usuario_registro { get; set; }
}
