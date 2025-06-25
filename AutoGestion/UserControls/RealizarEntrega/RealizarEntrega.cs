using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Servicios.Pdf;


namespace AutoGestion.Vista
{
    public partial class RealizarEntrega : UserControl
    {
        private readonly EntregaController _ctrl = new();
        private List<VentaDto> _ventas;

        public RealizarEntrega()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void CargarVentas()
        {
            // Pedimos los DTOs al controller
            _ventas = _ctrl.ObtenerVentasParaEntrega();
            dgvVentas.DataSource = null;
            dgvVentas.DataSource = _ventas;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnConfirmarEntrega_Click_1(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar una venta.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtenemos el DTO seleccionado
            var dto = dgvVentas.CurrentRow.DataBoundItem as VentaDto;
            if (dto == null) return;

            // Diálogo para elegir ruta de PDF
            using var dialogo = new SaveFileDialog
            {
                Filter = "Archivo PDF (*.pdf)|*.pdf",
                FileName = $"Comprobante_Entrega_{dto.ID}.pdf"
            };
            if (dialogo.ShowDialog() != DialogResult.OK) return;

            // Marcamos la entrega en BLL
            _ctrl.ConfirmarEntrega(dto.ID);

            // Recuperamos la entidad completa sólo para el PDF
            var ventaEntity = _ctrl.ObtenerEntidad(dto.ID);
            GeneradorComprobantePDF.Generar(ventaEntity, dialogo.FileName);

            MessageBox.Show("Entrega registrada y comprobante guardado correctamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refrescamos la lista
            CargarVentas();
        }



    }
}
