using GestionCobranza.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestionCobranza.Infrastructure.Services;

/// <summary>
/// Implementación de email por consola/log.
/// Reemplazar con implementación SMTP (MailKit) en producción.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    public EmailService(ILogger<EmailService> logger) => _logger = logger;

    public Task EnviarBienvenidaAsesorAsync(string correoDestino, string nombreCompleto, string contrasenaTemp)
    {
        _logger.LogInformation(
            "[EMAIL] Bienvenida enviada a {Correo} | Asesor: {Nombre} | Contraseña temporal: {Pass}",
            correoDestino, nombreCompleto, contrasenaTemp
        );

        // TODO: Integrar MailKit / SendGrid para envío real en producción
        // Ejemplo del cuerpo del correo:
        // "Hola {nombreCompleto}, tu cuenta en AUDICOB ha sido creada.
        //  Tu contraseña temporal es: {contrasenaTemp}
        //  Por seguridad, cámbiala al iniciar sesión por primera vez."

        return Task.CompletedTask;
    }
}
