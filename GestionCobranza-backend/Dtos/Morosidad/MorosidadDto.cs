using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionCobranza_backend.Dtos.Morosidad;

public class DashboardMorosidadDto
{
    public int ClientesMorosos { get; set; }
    public decimal DeudaMorosaTotal { get; set; }
    public int MorosidadCritica { get; set; }
    public int PromedioAtrasoDias { get; set; }
    public List<ClienteMorosoListaDto> DetalleClientes { get; set; } = new();
}

public class ClienteMorosoListaDto
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string AsesorAsignado { get; set; } = string.Empty;
    public int DiasAtraso { get; set; }
    public decimal DeudaPendiente { get; set; }
    public string Riesgo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}