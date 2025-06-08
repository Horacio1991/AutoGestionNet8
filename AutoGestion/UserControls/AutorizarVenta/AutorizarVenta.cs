using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;

namespace AutoGestion.Vista
{
    public partial class AutorizarVenta : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        private List<Venta> _ventasPendientes = new();

        public AutorizarVenta()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            _ventasPendientes = _ventaBLL.ObtenerVentasConEstadoPendiente();

            List<VentaVista> vista = _ventasPendientes.Select(v => new VentaVista
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

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            var vistaSeleccionada = dgvVentas.CurrentRow?.DataBoundItem as VentaVista;
            if (vistaSeleccionada == null) return;

            var ventaReal = _ventasPendientes.FirstOrDefault(v => v.ID == vistaSeleccionada.ID);

            if (ventaReal == null)
            {
                MessageBox.Show("No se pudo encontrar la venta.");
                return;
            }

            bool exito = _ventaBLL.AutorizarVenta(ventaReal.ID);

            if (exito)
                MessageBox.Show("Venta autorizada.");
            else
                MessageBox.Show("La venta fue rechazada automáticamente: el vehículo ya fue vendido.");

            CargarVentas();
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            var vistaSeleccionada = dgvVentas.CurrentRow?.DataBoundItem as VentaVista;
            if (vistaSeleccionada == null) return;

            string motivo = txtMotivoRechazo.Text.Trim();
            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Debe ingresar el motivo del rechazo.");
                return;
            }

            var ventaReal = _ventasPendientes.FirstOrDefault(v => v.ID == vistaSeleccionada.ID);

            if (ventaReal == null)
            {
                MessageBox.Show("No se pudo encontrar la venta.");
                return;
            }

            bool exito = _ventaBLL.RechazarVenta(ventaReal.ID, motivo);

            if (exito)
                MessageBox.Show("Venta rechazada.");
            else
                MessageBox.Show("No se pudo rechazar la venta.");

            CargarVentas();
        }
    }
}
