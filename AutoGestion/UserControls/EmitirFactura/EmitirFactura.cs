using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Servicios.Pdf;

namespace AutoGestion.Vista
{
    public partial class EmitirFactura : UserControl
    {
        private readonly VentaController _ventaController = new();
        private readonly FacturaController _facturaController = new();
        private List<VentaDto> _ventasParaFacturar;


        public EmitirFactura()
        {
            InitializeComponent();
            CargarVentas();
        }

        // Carga el datagrid con las ventas que ya estan autorizadas
        private void CargarVentas()
        {
            _ventasParaFacturar = _ventaController
                .ObtenerVentasParaFacturar();

            dgvVentas.DataSource = null;
            dgvVentas.DataSource = _ventasParaFacturar;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        //Toma la venta seleccionada, la graba y la marca como facturada. Genera un PDF con la factura.
        private void btnEmitir_Click_1(object sender, EventArgs e)
        {
            // 1) Validar selección
            var dto = dgvVentas.CurrentRow?.DataBoundItem as VentaDto;
            if (dto == null)
            {
                MessageBox.Show(
                    "Seleccione una venta para emitir la factura.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // 2) Mostrar vista previa
            string resumen =
                $"Cliente: {dto.Cliente}\n" +
                $"Vehículo: {dto.Vehiculo}\n" +
                $"Forma de Pago: {dto.TipoPago}\n" +
                $"Precio: ${dto.Monto:N2}\n" +
                $"Fecha: {dto.Fecha}";
            var resultado = MessageBox.Show(
                resumen + "\n\n¿Desea emitir esta factura?",
                "Vista previa de factura",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (resultado != DialogResult.Yes) return;

            try
            {
                // 3) Emitir la factura (persistir + marcar venta)
                var factura = _facturaController.EmitirFactura(dto.ID);

                // 4) Guardar PDF
                using var dlg = new SaveFileDialog
                {
                    Filter = "Archivo PDF (*.pdf)|*.pdf",
                    FileName = $"Factura_{factura.ID}.pdf"
                };
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                GeneradorFacturaPDF.Generar(factura, dlg.FileName);

                MessageBox.Show(
                    "Factura emitida correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // 5) Recargar listado (la venta ya no estará “Autorizada”)
                CargarVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al emitir la factura: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
