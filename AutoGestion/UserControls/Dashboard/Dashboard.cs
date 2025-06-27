using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;
using System.Windows.Forms.DataVisualization.Charting;
namespace Vista.UserControls.Dashboard
{
    public partial class Dashboard : UserControl
    {
        private readonly DashboardController _ctrl = new();

        public Dashboard()
        {
            InitializeComponent();

            cmbFiltroPeriodo.Items.AddRange(new object[]
            {
                "Hoy", "Últimos 7 días", "Últimos 30 días"
            });
            cmbFiltroPeriodo.SelectedIndex = 0;
            AplicarFiltro();
        }

        private void cmbFiltroPeriodo_SelectedIndexChanged(object sender, EventArgs e)
            => AplicarFiltro();


        // Determina el rango de fechas según el filtro seleccionado
        // y actualiza ambos gráficos (ventas y ranking).

         private void AplicarFiltro()
        {
            DateTime hoy = DateTime.Today, desde, hasta;
            switch (cmbFiltroPeriodo.SelectedItem as string)
            {
                case "Hoy":
                    desde = hasta = hoy; break;
                case "Últimos 7 días":
                    desde = hoy.AddDays(-6); hasta = hoy; break;
                case "Últimos 30 días":
                    desde = hoy.AddDays(-29); hasta = hoy; break;
                default:
                    desde = hasta = hoy; break;
            }

            // 1) Total facturado
            var total = _ctrl.ObtenerTotalFacturado(desde, hasta);
            lblTotalFacturado.Text = $"{ObtenerTextoPeriodo()}: {total:C2}";

            // 2) Ventas por día
            var ventas = _ctrl.ObtenerVentasFiltradas(desde, hasta);
            dgvVentas.DataSource = ventas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 3) Rango de fechas para el gráfico
            CargarGraficoVentas(ventas);

            // 4) Ranking
            var ranking = _ctrl.ObtenerRanking(desde, hasta);
            dgvRanking.DataSource = ranking;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            CargarGraficoRanking(ranking);
        }

        private void CargarGraficoVentas(List<DashboardVentaDto> ventas)
        {
            chartVentas.Series.Clear();
            var serie = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };
            chartVentas.ChartAreas[0].AxisX.CustomLabels.Clear();

            var porDia = ventas
                .GroupBy(v => v.Fecha)
                .Select(g => new { Fecha = g.Key, Total = g.Sum(x => x.Total) })
                .ToList();

            for (int i = 0; i < porDia.Count; i++)
            {
                var item = porDia[i];
                serie.Points.AddXY(i + 1, item.Total);
                // Tooltip con fecha y total
                serie.Points[i].ToolTip = $"{item.Fecha}: {item.Total:C2}";
                chartVentas.ChartAreas[0].AxisX.CustomLabels.Add(
                    new CustomLabel(i + 0.5, i + 1.5, item.Fecha, 0, LabelMarkStyle.None)
                );
            }

            chartVentas.Series.Add(serie);
            chartVentas.ChartAreas[0].AxisX.Interval = 1;
            chartVentas.Legends[0].Enabled = false;
        }


        private void CargarGraficoRanking(List<DashboardRankingDto> ranking)
        {
            chartRanking.Series.Clear();
            var serie = new Series("Ranking")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true
            };
            chartRanking.ChartAreas[0].AxisY.CustomLabels.Clear();

            for (int i = 0; i < ranking.Count; i++)
            {
                var item = ranking[i];
                // Añadimos la barra:
                serie.Points.AddXY(i + 1, item.Total);
                // Y establecemos el tooltip:
                serie.Points[i].ToolTip = $"{item.Vendedor}: {item.Total:C2}";

                // Seguimos poniendo la etiqueta en el eje Y:
                chartRanking.ChartAreas[0].AxisY.CustomLabels.Add(
                    new CustomLabel(i + 0.5, i + 1.5, item.Vendedor, 0, LabelMarkStyle.None)
                );
            }

            chartRanking.Series.Add(serie);
            chartRanking.ChartAreas[0].AxisY.Interval = 1;
            chartRanking.Legends[0].Enabled = false;
        }


        // Traduce el filtro seleccionado en el combo a un texto para el label.

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
