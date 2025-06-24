using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;
using AutoGestion.Servicios.Pdf;


namespace AutoGestion.Vista
{
    public partial class RealizarEntrega : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        //Guarda en memoria las ventas que ya fueron facturadas
        private List<Venta> _ventasFacturadas = new();

        public RealizarEntrega()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            // Carga las ventas facturadas desde el BLL
            _ventasFacturadas = _ventaBLL.ObtenerVentasFacturadas();

            // Transforma las ventas a una lista de vistas para el DataGridView
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

            // Obtener el objeto modelado de la fila seleccionada
            var seleccion = dgvVentas.CurrentRow.DataBoundItem as VentaVista;
            // Recuperar la venta original usando el ID
            var venta = _ventasFacturadas.FirstOrDefault(v => v.ID == seleccion.ID);

            if (venta == null)
            {
                MessageBox.Show("No se pudo encontrar la venta.");
                return;
            }

            // Mostrar diálogo para elegir dónde guardar el comprobante
            using SaveFileDialog dialogo = new SaveFileDialog
            {
                Filter = "Archivo PDF (*.pdf)|*.pdf",
                FileName = $"Comprobante_Entrega_{venta.ID}.pdf"
            };

            if (dialogo.ShowDialog() != DialogResult.OK)
                return;

            string rutaDestino = dialogo.FileName;

            // Marcar como entregada
            _ventaBLL.MarcarComoEntregada(venta.ID);

            // Generar comprobante en PDF
            GeneradorComprobantePDF.Generar(venta, rutaDestino);

            MessageBox.Show("Entrega registrada y comprobante guardado correctamente.");
            CargarVentas();
        }



    }
}
