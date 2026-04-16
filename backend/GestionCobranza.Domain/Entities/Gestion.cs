using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCobranza.Domain.Entities
{
    [Table("gestion_cobranza", Schema = "public")]
    public class Gestion
    {
        [Key]
        public int id_gestion { get; set; }
        public int id_cliente { get; set; }
        public string tipo_gestion { get; set; } = string.Empty; 
        public string descripcion { get; set; } = string.Empty;
        public DateTime fecha_gestion { get; set; }
        public string? resultado { get; set; }
        public bool eliminado { get; set; }

        [ForeignKey("id_cliente")]
        public virtual Cliente Cliente { get; set; } = null!;
    }
}