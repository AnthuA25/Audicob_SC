using System.ComponentModel.DataAnnotations;

namespace GestionCobranza.Application.DTOs;

public class EditarAsesorRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150)]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(150)]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [MaxLength(150)]
    public string Correo { get; set; } = null!;

    [MaxLength(15)]
    public string? Telefono { get; set; }
}
