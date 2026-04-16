using System.ComponentModel.DataAnnotations;

namespace GestionCobranza.Application.DTOs;

public class CrearAsesorRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El apellido no puede superar 150 caracteres.")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [MaxLength(150)]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos numéricos.")]
    public string Dni { get; set; } = null!;

    [MaxLength(15)]
    public string? Telefono { get; set; }
}
