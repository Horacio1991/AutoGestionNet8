using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;

namespace AutoGestion.Vista
{
    public partial class TasarVehiculo : UserControl
    {
        private readonly TasacionController _ctrl = new();
        private List<TasacionListDto> _ofertas;
        public TasarVehiculo()
        {
            InitializeComponent();

            // llenamos el combo y los estados
            cmbEstadoStock.Items.AddRange(new[] { "Disponible", "Requiere reacondicionamiento" });
            CargarOfertas();
            // configuramos DisplayMember/ValueMember
            //cmbOfertas.DisplayMember = nameof(TasacionListDto.VehiculoResumen);
            //cmbOfertas.ValueMember = nameof(TasacionListDto.OfertaID);
            //cmbOfertas.SelectedIndex = -1;
        }

        private void CargarOfertas()
        {
            // 1) Recuperamos la lista completa de DTOs
            _ofertas = _ctrl.ObtenerOfertasParaTasar();

            // 2) Enlazamos directamente al combo
            cmbOfertas.DataSource = null;
            cmbOfertas.DataSource = _ofertas;
            cmbOfertas.DisplayMember = nameof(TasacionListDto.VehiculoResumen);
            cmbOfertas.ValueMember = nameof(TasacionListDto.OfertaID);

            // 3) Reiniciamos selección y campos
            cmbOfertas.SelectedIndex = -1;
            txtEvaluacion.Clear();
            txtRango.Clear();
            txtValorFinal.Clear();
            cmbEstadoStock.SelectedIndex = -1;
        }


        private void cmbOfertas_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Si no hay selección, salgo
            if (cmbOfertas.SelectedItem is not TasacionListDto dto)
            {
                txtEvaluacion.Clear();
                txtRango.Clear();
                return;
            }

            // 4) Vuelco evaluación
            txtEvaluacion.Text =
                $"Motor: {dto.EstadoMotor}\r\n" +
                $"Carrocería: {dto.EstadoCarroceria}\r\n" +
                $"Interior: {dto.EstadoInterior}\r\n" +
                $"Documentación: {dto.EstadoDocumentacion}";

            // 5) Vuelco rango
            if (dto.RangoMin.HasValue && dto.RangoMax.HasValue)
                txtRango.Text = $"Entre {dto.RangoMin:C} y {dto.RangoMax:C}";
            else
                txtRango.Text = "Sin valores de referencia";
        }


        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            // 6) Validar selección
            if (cmbOfertas.SelectedItem is not TasacionListDto dto)
            {
                MessageBox.Show("Seleccione una oferta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 7) Validar valor final
            if (!decimal.TryParse(txtValorFinal.Text.Trim(), out var valorFinal))
            {
                MessageBox.Show("Ingrese un valor válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 8) Validar estado stock
            if (cmbEstadoStock.SelectedItem is not string estado)
            {
                MessageBox.Show("Seleccione el estado de stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 9) Preparo DTO y llamo controller
            try
            {
                var input = new TasacionInputDto
                {
                    OfertaID = dto.OfertaID,
                    ValorFinal = valorFinal,
                    EstadoStock = estado
                };
                _ctrl.RegistrarTasacion(input);

                MessageBox.Show("Tasación registrada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 10) Refrescar lista
                CargarOfertas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al tasar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
