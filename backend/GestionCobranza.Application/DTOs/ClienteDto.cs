using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionCobranza.Application.DTOs;

public record ClienteDto(
    int IdCliente,
    string NombreCompleto,
    string Dni,
    string Correo,
    string Telefono,     // <-- Asegúrate de que este esté aquí
    string Riesgo,
    string Estado
);