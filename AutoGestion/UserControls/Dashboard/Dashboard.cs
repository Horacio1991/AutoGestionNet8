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
            cmbFiltroPeriodo.Items.AddRange(new object[]
            {
                "Hoy",
                "Últimos 7 días",
                "Últimos 30 días"
            });
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
            // Filtrar ventas válidas en el rango
            var ventas = _ventaBLL.ObtenerTodas()
                .Where(v =>
                    (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                    v.Fecha.Date >= desde && v.Fecha.Date <= hasta
                )
                .ToList();

            // Cargar el DataGridView
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

            // Total facturado
            decimal total = ventas.Sum(v => v.Total);
            lblTotalFacturado.Text = $"{ObtenerTextoPeriodo()}: ${total:N2}";

            // Agrupar ventas por fecha
            var agrupadas = ventas
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Fecha = g.Key.ToString("dd/MM/yyyy"),
                    Total = g.Sum(v => v.Total)
                })
                .ToList();

            // Limpiar y crear la serie
            chartVentas.Series.Clear();
            chartVentas.ChartAreas[0].AxisX.CustomLabels.Clear();

            var serie = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = Color.SkyBlue
            };

            int index = 1;
            foreach (var item in agrupadas)
            {
                serie.Points.AddXY(index, item.Total);
                serie.Points[index - 1].ToolTip = $"{item.Fecha}: ${item.Total:N0}";

                // Agregar etiqueta personalizada debajo del número
                var label = new CustomLabel
                {
                    FromPosition = index - 0.5,
                    ToPosition = index + 0.5,
                    Text = item.Fecha
                };
                chartVentas.ChartAreas[0].AxisX.CustomLabels.Add(label);

                index++;
            }

            chartVentas.Series.Add(serie);
            // Asegurar que todas las barras tengan el mismo ancho visual
            serie["PointWidth"] = "0.2"; // Ajustable entre 0.0 y 1.0 (0.6 es un buen valor)

            // Evitar que el gráfico agregue espacio automático
            chartVentas.ChartAreas[0].AxisX.IsMarginVisible = true;


            // Configuración visual del gráfico
            var area = chartVentas.ChartAreas[0];
            area.AxisX.Title = "Día con ventas";
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = 0;

            area.AxisY.Title = "Monto Vendido ($)";
            area.AxisY.LabelStyle.Format = "C0";

            chartVentas.Legends[0].Enabled = false; // Oculta leyenda si no la querés
        }







        private void CargarRanking(DateTime desde, DateTime hasta)
        {
            var ventas = _ventaBLL.ObtenerTodas()
                .Where(v =>
                    (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                    v.Fecha.Date >= desde && v.Fecha.Date <= hasta
                )
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

            dgvRanking.DataSource = ranking;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRanking.ReadOnly = true;
            dgvRanking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

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
