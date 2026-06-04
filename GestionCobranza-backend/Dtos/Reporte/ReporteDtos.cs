using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Dtos.Reporte
{
    public class ReporteGerencialDto
    {
        public List<RendimientoAsesorDto> RendimientoAsesores { get; set; } = new();
        public List<ResumenClienteDto> ResumenClientes { get; set; } = new();
        public List<ReporteRecienteDto> ReportesRecientes { get; set; } = new();
    }

    public class ReporteRendimientoIndividualDto
    {
        public ResumenAsesorDto ResumenCartera { get; set; } = new();
        public List<ResumenClienteDto> DistribucionClientes { get; set; } = new();
        public List<ReporteRecienteDto> ReportesRecientes { get; set; } = new();
    }

    public class RendimientoAsesorDto
    {
        public string Asesor { get; set; } = string.Empty;
        public int Clientes { get; set; }
        public decimal DeudaGestionada { get; set; }
        public decimal PagosRecuperados { get; set; }
        public string Eficiencia { get; set; } = "0%";
    }

    public class ResumenAsesorDto
    {
        public int TotalClientesAsignados { get; set; }
        public decimal TotalDeudaAsignada { get; set; }
        public decimal TotalPagosRecuperados { get; set; }
        public string EficienciaIndividual { get; set; } = "0%";
    }

    public class ResumenClienteDto
    {
        public string Estado { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal DeudaTotal { get; set; }
        public double Porcentaje { get; set; }
    }

    public class ReporteRecienteDto
    {
        public int IdReporte { get; set; }
        public string NombreReporte { get; set; } = string.Empty;
        public string TipoReporte { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
        public string ArchivoUrl { get; set; } = string.Empty;
    }

    public class GenerarReporteRequestDto
    {
        public string TipoReporte { get; set; } = "Reporte General";
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class ReporteGeneradoResponseDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public int IdReporte { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UrlDescarga { get; set; } = string.Empty;
    }
}