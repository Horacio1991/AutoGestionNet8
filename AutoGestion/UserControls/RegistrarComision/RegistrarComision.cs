using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class RegistrarComision : UserControl
    {
        private readonly ComisionController _ctrl = new();
        private List<VentaComisionDto> _ventas;

        public RegistrarComision()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            _ventas = _ctrl.ObtenerVentasSinComision();
            dgvVentas.DataSource = null;
            dgvVentas.DataSource = _ventas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnConfirmar_Click_1(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaComisionDto dto)
            {
                MessageBox.Show("Seleccione una venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtComisionFinal.Text.Trim(), out var monto))
            {
                MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var input = new ComisionInputDto
            {
                VentaID = dto.VentaID,
                Monto = monto,
                Estado = "Aprobada"
            };

            try
            {
                _ctrl.RegistrarComision(input);
                MessageBox.Show("Comisión aprobada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComisionFinal.Clear();
                CargarVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aprobar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRechazar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is not VentaComisionDto dto)
            {
                MessageBox.Show("Seleccione una venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var motivo = txtMotivoRechazo.Text.Trim();
            if (motivo.Length == 0)
            {
                MessageBox.Show("Ingrese motivo de rechazo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var input = new ComisionInputDto
            {
                VentaID = dto.VentaID,
                Monto = 0m,
                Estado = "Rechazada",
                MotivoRechazo = motivo
            };

            try
            {
                _ctrl.RegistrarComision(input);
                MessageBox.Show("Comisión rechazada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMotivoRechazo.Clear();
                CargarVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al rechazar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
