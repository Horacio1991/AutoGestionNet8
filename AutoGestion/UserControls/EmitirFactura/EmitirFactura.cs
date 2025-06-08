using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;
using System;
using System.Linq;
using System.Windows.Forms;
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

        private void CargarVentas()
        {
            var ventas = _ventaBLL.ObtenerVentasPendientes()
                                  .Where(v => v.Estado == "Autorizada")
                                  .ToList();

            _ventasOriginales = ventas;

            var vistas = ventas.Select(v => VentaVista.DesdeVenta(v)).ToList();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = vistas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void btnEmitir_Click_1(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una venta para emitir la factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = dgvVentas.CurrentRow.Index;
            var venta = _ventasOriginales[index];

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
                _facturaBLL.EmitirFactura(factura);
                _ventaBLL.MarcarComoFacturada(venta.ID);

                // Fix: Provide a valid path for the "rutaDestino" parameter
                string rutaDestino = @"C:\Facturas\" + $"Factura_{factura.ID}.pdf";
                GeneradorFacturaPDF.Generar(factura, rutaDestino);

                MessageBox.Show("Factura emitida correctamente.");
                CargarVentas();
            }
        }

       
    }
}
