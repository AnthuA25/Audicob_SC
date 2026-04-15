using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionCobranza.Application.DTOs;

public record ClienteDto(int Id, string NombreCompleto, string Dni, string Correo, string Riesgo, string Estado);