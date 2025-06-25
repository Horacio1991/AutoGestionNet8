using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;

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

        private void CargarVentas()
        {
            _ventas = _ctrl.ObtenerVentasPendientes();
            dgvVentas.DataSource = _ventas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            // Obtenemos el DTO seleccionado
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDto dto) return;

            bool ok = _ctrl.AutorizarVenta(dto.ID);
            MessageBox.Show(ok
                ? "✅ Venta autorizada."
                : "❌ La venta fue rechazada: el vehículo ya está vendido.",
                "Autorizar venta",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            CargarVentas();
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaDto dto) return;

            string motivo = txtMotivoRechazo.Text.Trim();
            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Por favor, ingresa un motivo.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            bool ok = _ctrl.RechazarVenta(dto.ID, motivo);
            MessageBox.Show(ok
                ? "✅ Venta rechazada."
                : "❌ No se pudo rechazar la venta.",
                "Rechazar venta",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            CargarVentas();
        }
    }
}
