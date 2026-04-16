using GestionCobranza.Domain.Entities;

namespace GestionCobranza.Application.Interfaces;

public interface IDashboardRepository
{
    // Métricas de resumen (tarjetas)
    Task<int>     ContarClientesActivosAsync(int idAsesor);
    Task<int>     ContarClientesNuevosMesActualAsync(int idAsesor);
    Task<decimal> SumarSaldoPendienteAsync(int idAsesor);
    Task<decimal> SumarSaldoPendienteMesAnteriorAsync(int idAsesor);
    Task<decimal> SumarPagosConfirmadosAsync(int idAsesor);
    Task<decimal> SumarPagosConfirmadosMesAnteriorAsync(int idAsesor);
    Task<int>     ContarClientesMorososAsync(int idAsesor);
    Task<int>     ContarClientesMorososSemanaAnteriorAsync(int idAsesor);

    // Distribución mensual de clientes (últimos 6 meses)
    Task<List<(int Year, int Month, int NuevosClientes, int ClientesConPago)>>
        ObtenerDistribucionMensualAsync(int idAsesor);

    // Tendencia de morosidad (últimos 6 meses)
    Task<List<(int Year, int Month, int Morosos)>>
        ObtenerTendenciaMorosidadAsync(int idAsesor);

    // Clasificación de deudores por días de atraso
    Task<List<Cliente>> ObtenerClientesConVencimientoAsync(int idAsesor);

    // Clientes que tuvieron pago en cada mes (para distribución)
    Task<List<(int Year, int Month, int ClienteId)>>
        ObtenerPagosClientesPorMesAsync(int idAsesor);
}
