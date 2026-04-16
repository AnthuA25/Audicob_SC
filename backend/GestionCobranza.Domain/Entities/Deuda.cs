using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GestionCobranza.Domain.Entities
{
    [Table("deuda", Schema = "public")]
    public class Deuda
    {
        [Key]
        public int id_deuda { get; set; } // serial4
        public int id_cliente { get; set; } // int4
        public decimal monto_total { get; set; } // numeric(10,2)
        public decimal monto_pagado { get; set; } // numeric(10,2)
        public decimal saldo_pendiente { get; set; } // numeric(10,2)
        public DateTime fecha_vencimiento { get; set; } // date
        public int dias_atraso { get; set; } // int4
        public bool eliminado { get; set; } // bool

        [ForeignKey("id_cliente")]
        public virtual Cliente Cliente { get; set; } = null!;
    }
}