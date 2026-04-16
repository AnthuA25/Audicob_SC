using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionCobranza.Application.DTOs
{
    public class ClienteDetalleDto
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Riesgo { get; set; } = string.Empty;
        public decimal DeudaTotal { get; set; }
        public List<GestionHistorialDto> Gestiones { get; set; } = new();
    }

    public class GestionHistorialDto
    {
        public DateTime Fecha { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
    }
}