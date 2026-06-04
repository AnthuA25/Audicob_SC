using System;
using System.Collections.Generic;

namespace GestionCobranza_backend.Dtos.Reporte
{
    public class ReporteRendimientoIndividualDto
    {
        public ResumenAsesorDto ResumenCartera { get; set; } = new();
        public List<ResumenClienteDto> DistribucionClientes { get; set; } = new();
    }

    public class ResumenAsesorDto
    {
        public int TotalClientesAsignados { get; set; }
        public decimal TotalDeudaAsignada { get; set; }
        public decimal TotalPagosRecuperados { get; set; }
        public string EficienciaIndividual { get; set; } = "0%";
    }
}