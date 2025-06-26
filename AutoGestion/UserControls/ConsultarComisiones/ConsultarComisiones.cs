using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Servicios;


namespace AutoGestion.Vista
{
    public partial class ConsultarComisiones : UserControl
    {
        private readonly ComisionBLL _comisionBLL = new();
        //Lista para almacenar las comisiones filtradas
        private List<Comision> _comisionesFiltradas;

        public ConsultarComisiones()
        {
            InitializeComponent();
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            txtVendedor.Text = Sesion.UsuarioActual.Nombre;
            cmbEstado.Items.AddRange(new[] { "Aprobada", "Rechazada" });
            cmbEstado.SelectedIndex = 0;
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
        }

        private void btnFiltrar_Click_1(object sender, EventArgs e) { }
        //{
        //    string estado = cmbEstado.SelectedItem.ToString();
        //    DateTime desde = dtpDesde.Value.Date;
        //    DateTime hasta = dtpHasta.Value.Date;

        //    _comisionesFiltradas = _comisionBLL.ObtenerComisionesPorVendedorYFiltros(
        //        Sesion.UsuarioActual.ID, estado, desde, hasta);

        //    dgvComisiones.DataSource = _comisionesFiltradas.Select(c => new
        //    {
        //        ID = c.ID,
        //        Fecha = c.Fecha.ToShortDateString(),
        //        Cliente = $"{c.Venta.Cliente.Nombre} {c.Venta.Cliente.Apellido}",
        //        Vehiculo = $"{c.Venta.Vehiculo.Marca} {c.Venta.Vehiculo.Modelo}",
        //        Monto = c.Monto,
        //        Estado = c.Estado
        //    }).ToList();

        //    dgvComisiones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //}

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (dgvComisiones.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvComisiones.CurrentRow.Cells["ID"].Value);
            var comision = _comisionesFiltradas.FirstOrDefault(c => c.ID == id);

            if (comision == null) return;

            if (comision.Estado == "Aprobada")
            {
                MessageBox.Show("✅ Comisión aprobada.");
            }
            else
            {
                MessageBox.Show($"❌ Comisión rechazada. Motivo: {comision.MotivoRechazo}");
            }
        }

        
    }
}
