using System.Security.Cryptography;
using System.Text;
using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;
using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IEmailService _emailService;

    public UsuarioService(IUsuarioRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    // HU-06: Crear asesor
    public async Task<CrearAsesorResponseDto> CrearAsesorAsync(CrearAsesorRequestDto request)
    {
        if (await _repository.ExisteDniAsync(request.Dni))
            throw new InvalidOperationException("Ya existe un usuario registrado con este DNI.");

        if (await _repository.ExisteCorreoAsync(request.Correo))
            throw new InvalidOperationException("Ya existe un usuario registrado con este email.");

        var contrasenaTemp = GenerarContrasenaTemporalAsync();
        var hashContrasena = HashearContrasena(contrasenaTemp);

        var asesor = new Usuario
        {
            nombres          = request.Nombres.Trim(),
            apellidos        = request.Apellidos.Trim(),
            dni              = request.Dni.Trim(),
            correo           = request.Correo.Trim().ToLower(),
            telefono         = request.Telefono?.Trim(),
            contrasena_hash  = hashContrasena,
            id_perfil        = 2, // 2 = Asesor
            activo           = true,
            eliminado        = false,
            fecha_registro   = DateTime.UtcNow,
            usuario_registro = "SISTEMA"
        };

        var creado = await _repository.CrearAsync(asesor);

        await _emailService.EnviarBienvenidaAsesorAsync(
            creado.correo,
            $"{creado.nombres} {creado.apellidos}",
            contrasenaTemp
        );

        return new CrearAsesorResponseDto
        {
            Id            = creado.id,
            NombreCompleto = $"{creado.nombres} {creado.apellidos}",
            Correo        = creado.correo,
            Mensaje       = "Asesor creado correctamente."
        };
    }

    // HU-06: Listar asesores
    public async Task<IEnumerable<AsesorDetalleDto>> ListarAsesoresAsync(string? filtro)
    {
        var asesores = await _repository.ObtenerAsesoresAsync(filtro);
        return asesores.Select(u => new AsesorDetalleDto
        {
            Id            = u.id,
            NombreCompleto = $"{u.nombres} {u.apellidos}",
            Dni           = u.dni,
            Correo        = u.correo,
            Telefono      = u.telefono,
            Activo        = u.activo
        });
    }

    // HU-06: Editar asesor
    public async Task<AsesorDetalleDto> EditarAsesorAsync(int id, EditarAsesorRequestDto request)
    {
        var asesor = await _repository.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Asesor no encontrado.");

        if (await _repository.ExisteCorreoAsync(request.Correo, excluirId: id))
            throw new InvalidOperationException("Ya existe un usuario registrado con este email.");

        asesor.nombres         = request.Nombres.Trim();
        asesor.apellidos       = request.Apellidos.Trim();
        asesor.correo          = request.Correo.Trim().ToLower();
        asesor.telefono        = request.Telefono?.Trim();
        asesor.fecha_modifica  = DateTime.UtcNow;
        asesor.usuario_modifica = "SISTEMA";

        var actualizado = await _repository.ActualizarAsync(asesor);

        return new AsesorDetalleDto
        {
            Id            = actualizado.id,
            NombreCompleto = $"{actualizado.nombres} {actualizado.apellidos}",
            Dni           = actualizado.dni,
            Correo        = actualizado.correo,
            Telefono      = actualizado.telefono,
            Activo        = actualizado.activo
        };
    }

    // HU-06: Eliminar asesor (soft delete)
    public async Task EliminarAsesorAsync(int id)
    {
        var existe = await _repository.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Asesor no encontrado.");

        await _repository.EliminarAsync(id);
    }

    // HU-06: Verificar DNI disponible
    public async Task<bool> VerificarDniDisponibleAsync(string dni, int? excluirId = null)
        => !await _repository.ExisteDniAsync(dni, excluirId);

    // HU-06: Verificar correo disponible
    public async Task<bool> VerificarCorreoDisponibleAsync(string correo, int? excluirId = null)
        => !await _repository.ExisteCorreoAsync(correo, excluirId);

    // Genera contraseña temporal de 10 caracteres (letras + números)
    private static string GenerarContrasenaTemporalAsync()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        var bytes = RandomNumberGenerator.GetBytes(10);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }

    // Hash PBKDF2 con salt (sin dependencias externas)
    private static string HashearContrasena(string contrasena)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            contrasena,
            salt,
            iterations: 100_000,
            HashAlgorithmName.SHA256,
            outputLength: 32
        );
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
}
