using GestionCobranza.Application.DTOs;
using GestionCobranza.Application.Interfaces;

namespace GestionCobranza.Application.Services;

public class DashboardService : IDashboardService
{
    private static readonly string[] NombresMeses =
        { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

    private readonly IDashboardRepository _repo;
    public DashboardService(IDashboardRepository repo) => _repo = repo;

    public async Task<DashboardAsesorDto> ObtenerDashboardAsync(int idAsesor)
    {
        // Ejecutar todas las consultas en paralelo para cumplir el límite de 2 segundos
        var tareas = await Task.WhenAll(
            _repo.ContarClientesActivosAsync(idAsesor),
            _repo.ContarClientesNuevosMesActualAsync(idAsesor),
            _repo.ContarClientesMorososAsync(idAsesor),
            _repo.ContarClientesMorososSemanaAnteriorAsync(idAsesor),
            Task.FromResult(0) // placeholder
        );

        var (totalClientes, nuevosEsteMes, morososActual, morososSemanaAnterior) =
            (tareas[0], tareas[1], tareas[2], tareas[3]);

        var tareasDecimal = await Task.WhenAll(
            _repo.SumarSaldoPendienteAsync(idAsesor),
            _repo.SumarSaldoPendienteMesAnteriorAsync(idAsesor),
            _repo.SumarPagosConfirmadosAsync(idAsesor),
            _repo.SumarPagosConfirmadosMesAnteriorAsync(idAsesor)
        );

        var (deudaActual, deudaMesAnterior, pagosActual, pagosMesAnterior) =
            (tareasDecimal[0], tareasDecimal[1], tareasDecimal[2], tareasDecimal[3]);

        // Calcular comparativos
        var comparativoDeuda = CalcularComparativoPorcentaje(deudaActual, deudaMesAnterior, "vs mes anterior");
        var comparativoPagos = CalcularComparativoPorcentaje(pagosActual, pagosMesAnterior, "este mes");
        var difMorosidad     = morososActual - morososSemanaAnterior;
        var comparativoMorosidad = difMorosidad >= 0
            ? $"+{difMorosidad} vs semana pasada"
            : $"{difMorosidad} vs semana pasada";

        // Datos para gráficas
        var distribucionRaw    = await _repo.ObtenerDistribucionMensualAsync(idAsesor);
        var tendenciaRaw       = await _repo.ObtenerTendenciaMorosidadAsync(idAsesor);
        var pagosClientesMes   = await _repo.ObtenerPagosClientesPorMesAsync(idAsesor);
        var clientesConVencimiento = await _repo.ObtenerClientesConVencimientoAsync(idAsesor);

        // Construir los últimos 6 meses en orden
        var ultimosSeisMeses = ObtenerUltimosSeisMeses();

        var distribucion = ultimosSeisMeses.Select(periodo =>
        {
            var datos = distribucionRaw.FirstOrDefault(d => d.Year == periodo.Year && d.Month == periodo.Month);
            var pagosEnMes = pagosClientesMes
                .Where(p => p.Year == periodo.Year && p.Month == periodo.Month)
                .Select(p => p.ClienteId)
                .Distinct()
                .Count();

            return new DistribucionMensualDto
            {
                Mes            = NombresMeses[periodo.Month - 1],
                NuevosClientes = datos.NuevosClientes,
                ClientesConPago = pagosEnMes
            };
        }).ToList();

        var tendencia = ultimosSeisMeses.Select(periodo =>
        {
            var datos = tendenciaRaw.FirstOrDefault(d => d.Year == periodo.Year && d.Month == periodo.Month);
            return new TendenciaMensualDto
            {
                Mes     = NombresMeses[periodo.Month - 1],
                Morosos = datos.Morosos
            };
        }).ToList();

        // Clasificación por días de atraso (calculado en memoria con fecha actual)
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var clasificacion = ClasificarDeudores(clientesConVencimiento, hoy);

        return new DashboardAsesorDto
        {
            Resumen = new ResumenAsesorDto
            {
                TotalClientes         = totalClientes,
                ComparativoClientes   = $"+{nuevosEsteMes} este mes",
                DeudaPendiente        = deudaActual,
                ComparativoDeuda      = comparativoDeuda,
                PagosRealizados       = pagosActual,
                ComparativoPagos      = comparativoPagos,
                ClientesMorosidad     = morososActual,
                ComparativoMorosidad  = comparativoMorosidad
            },
            DistribucionClientes = distribucion,
            TendenciaMorosidad   = tendencia,
            ClasificacionDeudores = clasificacion
        };
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static List<(int Year, int Month)> ObtenerUltimosSeisMeses()
    {
        var resultado = new List<(int, int)>();
        var fecha = DateTime.Today;
        for (int i = 5; i >= 0; i--)
        {
            var mes = fecha.AddMonths(-i);
            resultado.Add((mes.Year, mes.Month));
        }
        return resultado;
    }

    private static string CalcularComparativoPorcentaje(decimal actual, decimal anterior, string etiqueta)
    {
        if (anterior == 0)
            return actual > 0 ? $"+100% {etiqueta}" : "sin datos";

        var pct = Math.Round((actual - anterior) / anterior * 100, 0);
        return pct >= 0 ? $"+{pct}% {etiqueta}" : $"{pct}% {etiqueta}";
    }

    private static ClasificacionDeudoresDto ClasificarDeudores(
        List<GestionCobranza.Domain.Entities.Cliente> clientes, DateOnly hoy)
    {
        int alDia = 0, atrasoLeve = 0, morosidad = 0, critico = 0;

        foreach (var c in clientes)
        {
            if (c.fecha_vencimiento is null || c.saldo_pendiente <= 0)
            {
                alDia++;
                continue;
            }
            var dias = hoy.DayNumber - c.fecha_vencimiento.Value.DayNumber;
            if (dias <= 0)       alDia++;
            else if (dias <= 30) atrasoLeve++;
            else if (dias <= 60) morosidad++;
            else                 critico++;
        }

        var total = alDia + atrasoLeve + morosidad + critico;
        decimal Pct(int v) => total == 0 ? 0 : Math.Round((decimal)v / total * 100, 1);

        return new ClasificacionDeudoresDto
        {
            AlDia               = alDia,
            PorcentajeAlDia     = Pct(alDia),
            AtrasoLeve          = atrasoLeve,
            PorcentajeAtrasoLeve = Pct(atrasoLeve),
            Morosidad           = morosidad,
            PorcentajeMorosidad = Pct(morosidad),
            Critico             = critico,
            PorcentajeCritico   = Pct(critico)
        };
    }
}
