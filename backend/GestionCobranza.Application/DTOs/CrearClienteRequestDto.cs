using System.ComponentModel.DataAnnotations;

namespace GestionCobranza.Application.DTOs;

public class CrearClienteRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El apellido no puede superar 150 caracteres.")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos numéricos.")]
    public string Dni { get; set; } = null!;

    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [MaxLength(150)]
    public string? Correo { get; set; }

    [MaxLength(15)]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un asesor asignado.")]
    public int IdAsesor { get; set; }

    [Required(ErrorMessage = "La deuda pendiente es obligatoria.")]
    [Range(0.01, 9999999999.99, ErrorMessage = "La deuda pendiente debe ser un valor mayor a 0.")]
    public decimal DeudaPendiente { get; set; }

    public DateOnly? FechaPago { get; set; }
}
