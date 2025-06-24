using AutoGestion.BLL;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vista.UserControls.Dashboard
{
    public partial class Dashboard : UserControl
    {
        // Capa de negocio para obtener ventas
        private readonly VentaBLL _ventaBLL = new();

        public Dashboard()
        {
            InitializeComponent();

            // Configuramos el combo con los filtros de periodo
            cmbFiltroPeriodo.Items.AddRange(new object[]
            {
                "Hoy",
                "Últimos 7 días",
                "Últimos 30 días"
            });
            cmbFiltroPeriodo.SelectedIndex = 0; // Por defecto "Hoy"

            // Primera carga
            AplicarFiltro();
        }

        // Cuando cambia el filtro de periodo, recargamos todo
        private void cmbFiltroPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

       
        // Determina el rango de fechas según el filtro seleccionado
        // y actualiza ambos gráficos (ventas y ranking).
       
        private void AplicarFiltro()
        {
            DateTime hoy = DateTime.Today;
            DateTime desde, hasta;

            switch (cmbFiltroPeriodo.SelectedItem?.ToString())
            {
                case "Hoy":
                    desde = hasta = hoy;
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
                    desde = hasta = hoy;
                    break;
            }

            CargarVentas(desde, hasta);
            CargarRanking(desde, hasta);
        }

        #region Ventas por día

       
        // Carga la tabla y gráfico de ventas facturadas/entregadas
        // entre las fechas 'desde' y 'hasta'.
       
        private void CargarVentas(DateTime desde, DateTime hasta)
        {
            // 1. Traer sólo las ventas facturadas o entregadas dentro del rango
            var ventas = _ventaBLL.ObtenerTodas()
                .Where(v =>
                    (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                    v.Fecha.Date >= desde && v.Fecha.Date <= hasta
                )
                .ToList();

            // 2. Mostrar en el DataGridView
            dgvVentas.DataSource = ventas
                .Select(v => new
                {
                    Fecha = v.Fecha.ToShortDateString(),
                    Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}",
                    Vehículo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                    Total = v.Total
                })
                .ToList();
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 3. Total facturado (se muestra en un Label)
            decimal total = ventas.Sum(v => v.Total);
            lblTotalFacturado.Text = $"{ObtenerTextoPeriodo()}: ${total:N2}";

            // 4. Agrupar por fecha para el gráfico de barras
            var agrupadas = ventas
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Fecha = g.Key.ToString("dd/MM/yyyy"),
                    Total = g.Sum(v => v.Total)
                })
                .ToList();

            // 5. Preparamos el chart
            chartVentas.Series.Clear();
            chartVentas.ChartAreas[0].AxisX.CustomLabels.Clear();

            var serie = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = Color.SkyBlue
            };

            int idx = 1;
            foreach (var item in agrupadas)
            {
                // El eje X maneja índices numéricos; luego ponemos etiquetas manuales
                serie.Points.AddXY(idx, item.Total);
                serie.Points[idx - 1].ToolTip = $"{item.Fecha}: ${item.Total:N0}";

                // Etiqueta debajo de cada barra con la fecha
                chartVentas.ChartAreas[0].AxisX.CustomLabels.Add(new CustomLabel
                {
                    FromPosition = idx - 0.5,
                    ToPosition = idx + 0.5,
                    Text = item.Fecha
                });

                idx++;
            }

            chartVentas.Series.Add(serie);

            // Uniformizar ancho de columnas y márgenes
            serie["PointWidth"] = "0.2";
            chartVentas.ChartAreas[0].AxisX.IsMarginVisible = true;

            // Títulos y formato de ejes
            var areaV = chartVentas.ChartAreas[0];
            areaV.AxisX.Title = "Día con ventas";
            areaV.AxisX.Interval = 1;
            areaV.AxisX.LabelStyle.Angle = 0;

            areaV.AxisY.Title = "Monto Vendido ($)";
            areaV.AxisY.LabelStyle.Format = "C0";

            // Opcional: ocultar la leyenda
            chartVentas.Legends[0].Enabled = false;
        }

        #endregion

        #region Ranking de vendedores

      
        /// Muestra un ranking horizontal de facturación por vendedor
        /// dentro del mismo rango de fechas.
      
        private void CargarRanking(DateTime desde, DateTime hasta)
        {
            // 1. Mismo filtro de ventas
            var ventas = _ventaBLL.ObtenerTodas()
                .Where(v =>
                    (v.Estado == "Facturada" || v.Estado == "Entregada") &&
                    v.Fecha.Date >= desde && v.Fecha.Date <= hasta
                )
                .ToList();

            // 2. Agrupar por vendedor y ordenar de mayor a menor total
            var ranking = ventas
                .GroupBy(v => v.Vendedor?.Nombre)
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new
                {
                    Vendedor = g.Key,
                    Total = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // 3. Mostrar en la tabla
            dgvRanking.DataSource = ranking;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRanking.ReadOnly = true;
            dgvRanking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 4. Preparar gráfico de barras horizontales
            chartRanking.Series.Clear();
            chartRanking.ChartAreas[0].AxisY.CustomLabels.Clear();

            var serie = new Series("Facturación")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.MediumSeaGreen
            };

            int idx = 1;
            foreach (var item in ranking)
            {
                // Igual que antes, usamos índice numérico y etiquetas manuales
                serie.Points.AddXY(idx, item.Total);
                serie.Points[idx - 1].ToolTip = $"{item.Vendedor}: ${item.Total:N0}";

                chartRanking.ChartAreas[0].AxisY.CustomLabels.Add(new CustomLabel
                {
                    FromPosition = idx - 0.5,
                    ToPosition = idx + 0.5,
                    Text = item.Vendedor
                });

                idx++;
            }

            chartRanking.Series.Add(serie);

            // Configuración de ejes
            var areaR = chartRanking.ChartAreas[0];
            areaR.AxisX.Title = "Monto Vendido ($)";
            areaR.AxisX.LabelStyle.Format = "C0";

            areaR.AxisY.Title = "Vendedor";
            areaR.AxisY.Interval = 1;
            // Ocultamos las líneas y ticks para dejar sólo los nombres
            areaR.AxisY.LabelStyle.Enabled = false;
            areaR.AxisY.MajorTickMark.Enabled = false;
            areaR.AxisY.LineWidth = 0;

            chartRanking.Legends[0].Enabled = false;
        }

        #endregion

        
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
