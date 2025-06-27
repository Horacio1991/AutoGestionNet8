using AutoGestion.BLL;
using AutoGestion.DTOs;

namespace AutoGestion.CTRL_Vista
{
    public class DashboardController
    {
        private readonly VentaBLL _ventaBll = new();

        public decimal ObtenerTotalFacturado(DateTime desde, DateTime hasta)
        {
            return _ventaBll.ObtenerTodas()
                .Where(v => (v.Estado == "Facturada" || v.Estado == "Entregada")
                         && v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                .Sum(v => v.Total);
        }

        public List<DashboardVentaDto> ObtenerVentasFiltradas(DateTime desde, DateTime hasta)
        {
            return _ventaBll.ObtenerTodas()
                .Where(v => (v.Estado == "Facturada" || v.Estado == "Entregada")
                         && v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                .OrderBy(v => v.Fecha)
                .Select(v => new DashboardVentaDto
                {
                    Fecha = v.Fecha.ToShortDateString(),
                    Cliente = $"{v.Cliente.Nombre} {v.Cliente.Apellido}",
                    Vehiculo = $"{v.Vehiculo.Marca} {v.Vehiculo.Modelo} ({v.Vehiculo.Dominio})",
                    Total = v.Total
                })
                .ToList();
        }

        public List<DashboardRankingDto> ObtenerRanking(DateTime desde, DateTime hasta)
        {
            return _ventaBll.ObtenerTodas()
                .Where(v => (v.Estado == "Facturada" || v.Estado == "Entregada")
                         && v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                .GroupBy(v => v.Vendedor.Nombre)
                .Select(g => new DashboardRankingDto
                {
                    Vendedor = g.Key,
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.Total)
                .ToList();
        }
    }
}
