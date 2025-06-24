using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;


namespace AutoGestion.Vista
{
    public partial class RegistrarComision : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        private readonly ComisionBLL _comisionBLL = new();

        //lista en memoria de ventas sin comisión asignada
        private List<Venta> _ventasSinComision;

        public RegistrarComision()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            _ventasSinComision = _ventaBLL.ObtenerVentasSinComisionAsignada();

            List<VentaComisionVista> vista = _ventasSinComision.Select(v => new VentaComisionVista
            {
                ID = v.ID,
                Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}",
                Vendedor = v.Vendedor?.Nombre,
                Vehiculo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo}",
                MontoVenta = v.Total,
                ComisionSugerida = v.Total * 0.05m,
                Fecha = v.Fecha.ToShortDateString()
            }).ToList();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = vista;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnConfirmar_Click_1(object sender, EventArgs e)
        {
            var seleccion = dgvVentas.CurrentRow?.DataBoundItem as VentaComisionVista;
            if (seleccion == null) return;

            if (!decimal.TryParse(txtComisionFinal.Text, out decimal valorFinal))
            {
                MessageBox.Show("Ingrese un valor válido para la comisión.");
                return;
            }

            var venta = _ventasSinComision.FirstOrDefault(v => v.ID == seleccion.ID);

            if (venta == null)
            {
                MessageBox.Show("No se encontró la venta seleccionada.");
                return;
            }

            var comision = new Comision
            {
                Venta = venta,
                Porcentaje = valorFinal / venta.Total,
                Monto = valorFinal,
                Estado = "Aprobada"
            };

            _comisionBLL.Registrar(comision);
            MessageBox.Show("Comisión registrada correctamente.");

            CargarVentas();
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            var seleccion = dgvVentas.CurrentRow?.DataBoundItem as VentaComisionVista;
            if (seleccion == null) return;

            string motivo = txtMotivoRechazo.Text.Trim();
            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Debe ingresar el motivo del rechazo.");
                return;
            }

            var venta = _ventasSinComision.FirstOrDefault(v => v.ID == seleccion.ID);

            if (venta == null)
            {
                MessageBox.Show("No se encontró la venta seleccionada.");
                return;
            }

            var comision = new Comision
            {
                Venta = venta,
                Porcentaje = 0,
                Monto = 0,
                Estado = "Rechazada",
                MotivoRechazo = motivo
            };

            _comisionBLL.Registrar(comision);
            MessageBox.Show("Comisión rechazada correctamente.");

            CargarVentas();
            txtMotivoRechazo.Clear();  // Limpiar campo después
        }

        
    }
}
