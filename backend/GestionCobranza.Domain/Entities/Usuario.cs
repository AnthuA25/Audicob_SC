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

    [Required]
    public string dni { get; set; } = null!;

    [Required]
    public string correo { get; set; } = null!;

    public string? telefono { get; set; }

    [Required]
    public string contrasena_hash { get; set; } = null!;

    public bool activo { get; set; } = true;
    public bool eliminado { get; set; } = false;

    // id_perfil: 1 = Administrador, 2 = Asesor
    public int id_perfil { get; set; }

    public DateTime fecha_registro { get; set; } = DateTime.UtcNow;
    public string? usuario_registro { get; set; }

    public DateTime? fecha_modifica { get; set; }
    public string? usuario_modifica { get; set; }
}
