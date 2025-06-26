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

            // Creamos un anonimo para atar los nombres de columna que tenemos
            var tabla = _ventas
                .Select(v => new
                {
                    VentaID = v.VentaID,
                    Cliente = v.Cliente,
                    Vendedor = v.Vendedor,
                    Vehículo = v.VehiculoResumen,
                    MontoVenta = v.MontoVenta,
                    ComisionSug = v.ComisionSugerida,
                    FechaVenta = v.FechaVenta
                })
                .ToList();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = tabla;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.ReadOnly = true;
        }

        private void btnConfirmar_Click_1(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una fila.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recupero el ID de la venta
            int ventaId = Convert.ToInt32(dgvVentas.CurrentRow.Cells["VentaID"].Value);

            // Valido monto ingresado
            if (!decimal.TryParse(txtComisionFinal.Text.Trim(), out decimal monto))
            {
                MessageBox.Show("Ingrese un valor numérico válido para la comisión.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Armo el DTO y lo envío al controller
            var dto = new ComisionInputDto
            {
                VentaID = ventaId,
                Monto = monto,
                Estado = "Aprobada",
                MotivoRechazo = null
            };

            bool ok = _ctrl.RegistrarComision(dto);
            MessageBox.Show(ok
                ? "✅ Comisión aprobada."
                : "❌ Error al aprobar la comisión.");

            // Refrescar lista y limpiar inputs
            txtComisionFinal.Clear();
            txtMotivoRechazo.Clear();
            CargarVentas();
        }


        private void btnRechazar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una fila.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recupero ID de la venta
            int ventaId = Convert.ToInt32(dgvVentas.CurrentRow.Cells["VentaID"].Value);

            // Valido que informe un motivo
            string motivo = txtMotivoRechazo.Text.Trim();
            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Debe ingresar el motivo del rechazo.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dto = new ComisionInputDto
            {
                VentaID = ventaId,
                Monto = 0m,
                Estado = "Rechazada",
                MotivoRechazo = motivo
            };

            bool ok = _ctrl.RegistrarComision(dto);
            MessageBox.Show(ok
                ? "✅ Comisión rechazada."
                : "❌ Error al rechazar la comisión.");

            // Refrescar y limpiar
            txtComisionFinal.Clear();
            txtMotivoRechazo.Clear();
            CargarVentas();
        }

    }
}
