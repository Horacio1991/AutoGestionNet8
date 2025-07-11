using AutoGestion.BLL;
using AutoGestion.DTOs;

namespace AutoGestion.CTRL_Vista
{
    public class DashboardController
    {
        private readonly VentaBLL _ventaBll = new();
        public decimal ObtenerTotalFacturado(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'desde' no puede ser posterior a 'hasta'.");

            try
            {
                var ventas = _ventaBll.ObtenerTodas()
                    .Where(v =>
                        (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                        v.Fecha.Date >= desde.Date &&
                        v.Fecha.Date <= hasta.Date);

                return ventas.Sum(v => v.Total); // Sumar el total de todas las ventas filtradas
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al calcular total facturado: {ex.Message}", ex);
            }
        }


        // Obtiene el listado de ventas (estado “Facturada” o “Entregada”) formateadas
        public List<DashboardVentaDto> ObtenerVentasFiltradas(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'desde' no puede ser posterior a 'hasta'.");

            try
            {
                return _ventaBll.ObtenerTodas()
                    .Where(v =>
                        (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                        v.Fecha.Date >= desde.Date &&
                        v.Fecha.Date <= hasta.Date)
                    .OrderBy(v => v.Fecha)
                    .Select(v => new DashboardVentaDto
                    {
                        Fecha = v.Fecha.ToShortDateString(),
                        Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}".Trim(),
                        Vehiculo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                        Total = v.Total
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ventas filtradas: {ex.Message}", ex);
            }
        }

        // Calcula el ranking de vendedores, sumando el total facturado
        // por cada uno en el rango de fechas dado.
        public List<DashboardRankingDto> ObtenerRanking(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha 'desde' no puede ser posterior a 'hasta'.");

            try
            {
                return _ventaBll.ObtenerTodas()
                    .Where(v =>
                        (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                        v.Fecha.Date >= desde.Date &&
                        v.Fecha.Date <= hasta.Date)
                    .GroupBy(v => v.Vendedor?.Nombre ?? "Desconocido")
                    .Select(g => new DashboardRankingDto
                    {
                        Vendedor = g.Key,
                        Total = g.Sum(v => v.Total)
                    })
                    .OrderByDescending(d => d.Total)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ranking de vendedores: {ex.Message}", ex);
            }
        }
    }
}
