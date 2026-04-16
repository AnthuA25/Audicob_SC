namespace GestionCobranza.Application.Interfaces;

public interface IEmailService
{
    Task EnviarBienvenidaAsesorAsync(string correoDestino, string nombreCompleto, string contrasenaTemp);
}
