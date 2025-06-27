using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;
using AutoGestion.Servicios; // Para Sesion


namespace AutoGestion.Vista
{
    public partial class ConsultarComisiones : UserControl
    {
        private readonly ComisionController _ctrl = new ComisionController();
        private List<ComisionListDto> _comisionesFiltradas;

        public ConsultarComisiones()
        {
            InitializeComponent();

            // Inicializo filtros por defecto
            txtVendedor.Text = Sesion.UsuarioActual.Nombre;
            cmbEstado.Items.AddRange(new[] { "Aprobada", "Rechazada" });
            cmbEstado.SelectedIndex = 0;
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
        }

        private void btnFiltrar_Click_1(object sender, EventArgs e)
        {
            var estado = cmbEstado.SelectedItem.ToString();
            var desde = dtpDesde.Value.Date;
            var hasta = dtpHasta.Value.Date;
            var vendedorId = Sesion.UsuarioActual.ID;

            _comisionesFiltradas = _ctrl.ObtenerComisiones(
                vendedorId, estado, desde, hasta);

            dgvComisiones.DataSource = _comisionesFiltradas
                .Select(c => new
                {
                    c.ID,
                    c.Fecha,
                    c.Cliente,
                    c.Vehiculo,
                    c.Monto,
                    c.Estado
                })
                .ToList();

            dgvComisiones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComisiones.ReadOnly = true;
            dgvComisiones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (dgvComisiones.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvComisiones.CurrentRow.Cells["ID"].Value);
            var com = _comisionesFiltradas.FirstOrDefault(c => c.ID == id);
            if (com == null) return;

            if (com.Estado == "Aprobada")
                MessageBox.Show("✅ Comisión aprobada.");
            else
                MessageBox.Show($"❌ Comisión rechazada.\nMotivo: {com.MotivoRechazo}");
        }


    }
}
