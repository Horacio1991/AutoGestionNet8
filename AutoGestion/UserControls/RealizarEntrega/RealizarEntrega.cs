using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;
using AutoGestion.Servicios.Pdf;


namespace AutoGestion.Vista
{
    public partial class RealizarEntrega : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        private List<Venta> _ventasFacturadas = new();

        public RealizarEntrega()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            _ventasFacturadas = _ventaBLL.ObtenerVentasFacturadas();

            var vista = _ventasFacturadas.Select(v => new VentaVista
            {
                ID = v.ID,
                Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}",
                Vehiculo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                TipoPago = v.Pago?.TipoPago,
                Monto = v.Pago?.Monto ?? 0,
                Estado = v.Estado,
                Fecha = v.Fecha.ToShortDateString()
            }).ToList();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = vista;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnConfirmarEntrega_Click_1(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar una venta.");
                return;
            }

            var seleccion = dgvVentas.CurrentRow.DataBoundItem as VentaVista;
            var venta = _ventasFacturadas.FirstOrDefault(v => v.ID == seleccion.ID);

            if (venta == null)
            {
                MessageBox.Show("No se pudo encontrar la venta.");
                return;
            }

            // Marcar como entregada
            _ventaBLL.MarcarComoEntregada(venta.ID);

            // Generar comprobante en PDF
            var rutaDestino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Comprobante_{venta.ID}.pdf");
            GeneradorComprobantePDF.Generar(venta, rutaDestino);

            MessageBox.Show($"Entrega registrada y comprobante generado en: {rutaDestino}");
            CargarVentas();
        }

    
    }
}
