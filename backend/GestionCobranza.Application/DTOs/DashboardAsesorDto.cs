namespace GestionCobranza.Application.DTOs;

/// <summary>
/// Respuesta completa del dashboard para un asesor.
/// Cubre los 3 criterios de aceptación de HU-10.
/// </summary>
public class DashboardAsesorDto
{
    // CA-1: Cuatro tarjetas resumen
    public ResumenAsesorDto Resumen { get; set; } = null!;

    // CA-2: Gráfico de líneas - distribución mensual de clientes (últimos 6 meses)
    public List<DistribucionMensualDto> DistribucionClientes { get; set; } = new();

    // CA-2: Gráfico de barras - tendencia de morosidad (últimos 6 meses)
    public List<TendenciaMensualDto> TendenciaMorosidad { get; set; } = new();

    // CA-3: Gráfico circular - clasificación de deudores
    public ClasificacionDeudoresDto ClasificacionDeudores { get; set; } = null!;
}

/// <summary>CA-1: Métricas de las 4 tarjetas con comparativo.</summary>
public class ResumenAsesorDto
{
    public int    TotalClientes           { get; set; }
    public string ComparativoClientes     { get; set; } = string.Empty; // "+12 este mes"

    public decimal DeudaPendiente         { get; set; }
    public string  ComparativoDeuda       { get; set; } = string.Empty; // "-8% vs mes anterior"

    public decimal PagosRealizados        { get; set; }
    public string  ComparativoPagos       { get; set; } = string.Empty; // "+15% este mes"

    public int     ClientesMorosidad      { get; set; }
    public string  ComparativoMorosidad   { get; set; } = string.Empty; // "-5 vs semana pasada"
}

/// <summary>CA-2: Punto del gráfico de líneas (nuevos clientes vs clientes con pago por mes).</summary>
public class DistribucionMensualDto
{
    public string Mes              { get; set; } = null!; // "Ene", "Feb"...
    public int    NuevosClientes   { get; set; }
    public int    ClientesConPago  { get; set; }
}

/// <summary>CA-2: Punto del gráfico de barras de morosidad por mes.</summary>
public class TendenciaMensualDto
{
    public string Mes        { get; set; } = null!;
    public int    Morosos    { get; set; }
}

/// <summary>CA-3: Clasificación por días de atraso.</summary>
public class ClasificacionDeudoresDto
{
    public int     AlDia              { get; set; }
    public decimal PorcentajeAlDia    { get; set; }

    public int     AtrasoLeve         { get; set; }
    public decimal PorcentajeAtrasoLeve { get; set; }

    public int     Morosidad          { get; set; }
    public decimal PorcentajeMorosidad { get; set; }

    public int     Critico            { get; set; }
    public decimal PorcentajeCritico  { get; set; }
}
