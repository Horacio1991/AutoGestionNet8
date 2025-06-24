using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;
using AutoGestion.Servicios.Pdf;

namespace AutoGestion.Vista
{
    public partial class EmitirFactura : UserControl
    {
        private readonly VentaBLL _ventaBLL = new();
        private readonly FacturaBLL _facturaBLL = new();
        private List<Venta> _ventasOriginales;


        public EmitirFactura()
        {
            InitializeComponent();
            CargarVentas();
        }

        // Carga el datagrid con las ventas que ya estan autorizadas
        private void CargarVentas()
        {
            // Filtra las ventas autorizadas unicamente
            var ventas = _ventaBLL.ObtenerVentasPendientes()
                                  .Where(v => v.Estado == "Autorizada")
                                  .ToList();

            // Se guarda la lista original para despues localizar el objeto venta
            _ventasOriginales = ventas;

            var vistas = ventas.Select(v => VentaVista.DesdeVenta(v)).ToList();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = vistas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        //Toma la venta seleccionada, la graba y la marca como facturada. Genera un PDF con la factura.
        private void btnEmitir_Click_1(object sender, EventArgs e)
        {
            // Verifica que haya una fila seleccionada en el DataGridView
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una venta para emitir la factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // obtengo el indice de la fila seleccionada
            int index = dgvVentas.CurrentRow.Index;
            // Recuperamos la venta real a partir de la lista original
            var venta = _ventasOriginales[index];
            // Se crea la factura
            var factura = new Factura
            {
                Cliente = venta.Cliente,
                Vehiculo = venta.Vehiculo,
                Precio = venta.Total,
                Fecha = DateTime.Now,
                FormaPago = venta.Pago?.TipoPago ?? "Desconocido"
            };

            string resumen = $"Cliente: {factura.Cliente.Nombre} {factura.Cliente.Apellido}\n" +
                             $"Vehículo: {factura.Vehiculo.Marca} {factura.Vehiculo.Modelo} ({factura.Vehiculo.Dominio})\n" +
                             $"Forma de Pago: {factura.FormaPago}\n" +
                             $"Precio: ${factura.Precio}\n" +
                             $"Fecha: {factura.Fecha.ToShortDateString()}";

            DialogResult result = MessageBox.Show(
                resumen + "\n\n¿Desea emitir esta factura?",
                "Vista previa de factura",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                //Guarda la factura en el XML
                _facturaBLL.EmitirFactura(factura);
                // Marca la venta como facturada
                _ventaBLL.MarcarComoFacturada(venta.ID);

                // Abre el diálogo para guardar el PDF (el usuario elige la ubicación)
                using SaveFileDialog dialogo = new SaveFileDialog
                {
                    Filter = "Archivo PDF (*.pdf)|*.pdf",
                    FileName = $"Factura_{factura.ID}.pdf"
                };

                if (dialogo.ShowDialog() != DialogResult.OK)
                    return;

                string rutaDestino = dialogo.FileName;
                GeneradorFacturaPDF.Generar(factura, rutaDestino);

                MessageBox.Show("Factura emitida correctamente.");
                CargarVentas();
            }
        }

    }
}
