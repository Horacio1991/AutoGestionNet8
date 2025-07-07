using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.CTRL_Vista;

namespace AutoGestion.Vista
{
    public partial class AutorizarVenta : UserControl
    {
        private readonly VentaController _ctrl = new();
        private List<VentaDto> _ventas;

        public AutorizarVenta()
        {
            InitializeComponent();
            CargarVentas(); 
        }

        // Lee del BLL las ventas pendientes y las muestra en el DataGridView.
        private void CargarVentas()
        {
            try
            {
                _ventas = _ctrl.ObtenerVentasPendientes();
                dgvVentas.DataSource = null;
                dgvVentas.DataSource = _ventas;
                dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvVentas.ReadOnly = true;
                dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar ventas pendientes:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón Autorizar: cambia estado en BLL y refresca.
        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDto dto)
            {
                MessageBox.Show("Seleccione una venta para autorizar.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool autorizado = _ctrl.AutorizarVenta(dto.ID);
                var mensaje = autorizado
                    ? "✅ Venta autorizada."
                    : "❌ No se pudo autorizar: el vehículo ya está vendido.";
                var icono = autorizado ? MessageBoxIcon.Information : MessageBoxIcon.Warning;

                MessageBox.Show(mensaje, "Autorizar venta",
                                MessageBoxButtons.OK, icono);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al autorizar la venta:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CargarVentas();
            }
        }

        // solicita motivo, envía a la BLL y refresca.
        private void btnRechazar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDto dto)
            {
                MessageBox.Show("Seleccione una venta para rechazar.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string motivo = txtMotivoRechazo.Text.Trim();
            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Ingrese un motivo de rechazo.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                bool rechazado = _ctrl.RechazarVenta(dto.ID, motivo);
                var mensaje = rechazado
                    ? "✅ Venta rechazada."
                    : "❌ No se pudo rechazar la venta.";
                var icono = rechazado ? MessageBoxIcon.Information : MessageBoxIcon.Error;

                MessageBox.Show(mensaje, "Rechazar venta",
                                MessageBoxButtons.OK, icono);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al rechazar la venta:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CargarVentas();
            }
        }
    }
}
