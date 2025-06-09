using AutoGestion.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vista.UserControls.Dashboard
{
    public partial class Dashboard : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        public Dashboard()
        {
            InitializeComponent();
            cmbFiltroPeriodo.SelectedIndex = 0; // Default: Hoy
            AplicarFiltro();
        }

        private void cmbFiltroPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro();

        }
        private void AplicarFiltro()
        {
            DateTime hoy = DateTime.Today;
            DateTime desde = hoy;
            DateTime hasta = hoy;

            switch (cmbFiltroPeriodo.SelectedItem?.ToString())
            {
                case "Hoy":
                    desde = hoy;
                    hasta = hoy;
                    break;
                case "Últimos 7 días":
                    desde = hoy.AddDays(-6);
                    hasta = hoy;
                    break;
                case "Últimos 30 días":
                    desde = hoy.AddDays(-29);
                    hasta = hoy;
                    break;
                default:
                    desde = hoy;
                    hasta = hoy;
                    break;
            }

            CargarVentas(desde, hasta);
            CargarRanking(desde, hasta);
        }

        private void CargarVentas(DateTime desde, DateTime hasta)
        {
            var ventas = _ventaBLL.ObtenerVentasFacturadas()
                                  .Where(v => v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                                  .ToList();

            // Actualizar DataGridView
            dgvVentas.DataSource = ventas.Select(v => new
            {
                Fecha = v.Fecha.ToShortDateString(),
                Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}",
                Vehículo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                Total = v.Total
            }).ToList();

            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Calcular total
            decimal total = ventas.Sum(v => v.Total);
            lblTotalFacturado.Text = $"{ObtenerTextoPeriodo()}: ${total:N2}";

            // Gráfico
            chartVentas.Series.Clear();
            var serie = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            var agrupadas = ventas
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Fecha = g.Key.ToShortDateString(), Total = g.Sum(v => v.Total) });

            foreach (var dato in agrupadas)
                serie.Points.AddXY(dato.Fecha, dato.Total);

            chartVentas.Series.Add(serie);
            chartVentas.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
        }

        private void CargarRanking(DateTime desde, DateTime hasta)
        {
            var ventas = _ventaBLL.ObtenerVentasFacturadas()
                                  .Where(v => v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                                  .ToList();

            var ranking = ventas
                .GroupBy(v => v.Vendedor?.Nombre)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => new
                {
                    Vendedor = g.Key,
                    Cantidad = g.Count(),
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // DataGridView
            dgvRanking.DataSource = ranking;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRanking.ReadOnly = true;
            dgvRanking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Gráfico
            chartRanking.Series.Clear();
            var serie = new Series("Facturación")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true
            };

            foreach (var r in ranking)
                serie.Points.AddXY(r.Vendedor, r.Total);

            chartRanking.Series.Add(serie);
            chartRanking.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
        }

        private string ObtenerTextoPeriodo()
        {
            return cmbFiltroPeriodo.SelectedItem?.ToString() switch
            {
                "Hoy" => "Total del día",
                "Últimos 7 días" => "Total últimos 7 días",
                "Últimos 30 días" => "Total últimos 30 días",
                _ => "Total"
            };
        }



    }
}
