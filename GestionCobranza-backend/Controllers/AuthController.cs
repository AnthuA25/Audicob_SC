using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionCobranza_backend.Dtos.Auth;
using GestionCobranza_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GestionCobranza_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Dni) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new
                {
                    message = "DNI y contraseña son obligatorios."
                });
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u =>
                    u.Dni == dto.Dni.Trim() &&
                    u.Activo &&
                    !u.Eliminado
                );

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    message = "Credenciales incorrectas."
                });
            }

            if (usuario.Estado?.ToUpper() != "ACTIVO")
            {
                return Unauthorized(new
                {
                    message = "El usuario no está activo."
                });
            }

            if (string.IsNullOrEmpty(usuario.PasswordHash))
            {
                return StatusCode(500, new
                {
                    message = "El usuario no tiene contraseña registrada."
                });
            }

            var passwordValida = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                usuario.PasswordHash
            );

            if (!passwordValida)
            {
                return Unauthorized(new
                {
                    message = "Credenciales incorrectas."
                });
            }

            if (usuario.IdRolNavigation == null)
            {
                return StatusCode(500, new
                {
                    message = "El usuario no tiene rol asignado."
                });
            }

            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, new
                {
                    message = "JWT Key no configurada."
                });
            }

            if (string.IsNullOrEmpty(jwtIssuer))
            {
                return StatusCode(500, new
                {
                    message = "JWT Issuer no configurado."
                });
            }

            if (string.IsNullOrEmpty(jwtAudience))
            {
                return StatusCode(500, new
                {
                    message = "JWT Audience no configurado."
                });
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    $"{usuario.Nombres} {usuario.Apellidos}"
                ),

                new Claim(
                    ClaimTypes.Email,
                    usuario.Correo ?? ""
                ),

                new Claim(
                    ClaimTypes.Role,
                    usuario.IdRolNavigation.Nombre
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                message = "Login exitoso",
                token = tokenString,
                user = new
                {
                    usuario.IdUsuario,
                    usuario.Nombres,
                    usuario.Apellidos,
                    usuario.Correo,
                    usuario.Estado,
                    Rol = usuario.IdRolNavigation.Nombre
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error interno del servidor",
                error = ex.Message,
                detalle = ex.InnerException?.Message
            });
        }
    }
}