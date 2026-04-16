using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCobranza.Domain.Entities;

[Table("usuario", Schema = "public")]
public class Usuario
{
    [Key]
    public int id { get; set; }

    [Required]
    public string nombres { get; set; } = null!;

    [Required]
    public string apellidos { get; set; } = null!;

    public bool activo { get; set; } = true;
    public bool eliminado { get; set; } = false;

    public int id_perfil { get; set; }
}
