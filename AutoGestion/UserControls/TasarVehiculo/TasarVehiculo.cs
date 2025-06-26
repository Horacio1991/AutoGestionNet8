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
            CargarOfertas();
            cmbEstadoStock.Items.AddRange(new[] { "Disponible", "Requiere reacondicionamiento" });

            // configuramos DisplayMember/ValueMember
            cmbOfertas.DisplayMember = nameof(TasacionListDto.VehiculoResumen);
            cmbOfertas.ValueMember = nameof(TasacionListDto.OfertaID);
            cmbOfertas.SelectedIndex = -1;
        }

        private void CargarOfertas()
        {
            _ofertas = _ctrl.ObtenerOfertasParaTasar();

            cmbOfertas.DataSource = null;
            cmbOfertas.DataSource = _ofertas;

            // limpiamos los campos hasta seleccionar algo
            txtEvaluacion.Clear();
            txtRango.Clear();
            txtValorFinal.Clear();
            cmbEstadoStock.SelectedIndex = -1;
        }


        private void cmbOfertas_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedItem is not TasacionListDto dto)
            {
                txtEvaluacion.Clear();
                txtRango.Clear();
                return;
            }

            // volcamos TODOS los campos de evaluación
            txtEvaluacion.Text =
                $"Motor: {dto.EstadoMotor}\r\n" +
                $"Carrocería: {dto.EstadoCarroceria}\r\n" +
                $"Interior: {dto.EstadoInterior}\r\n" +
                $"Documentación: {dto.EstadoDocumentacion}";

            // sólo el rango en txtRango
            if (dto.RangoMin.HasValue && dto.RangoMax.HasValue)
                txtRango.Text = $"Entre {dto.RangoMin:C} y {dto.RangoMax:C}";
            else
                txtRango.Text = "Sin valores de referencia";
        }


        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedItem is not TasacionListDto dto)
            {
                MessageBox.Show("Seleccione una oferta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtValorFinal.Text.Trim(), out var valorFinal))
            {
                MessageBox.Show("Ingrese un valor válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbEstadoStock.SelectedItem is not string estado)
            {
                MessageBox.Show("Seleccione el estado de stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var input = new TasacionInputDto
                {
                    OfertaID = dto.OfertaID,
                    ValorFinal = valorFinal,
                    EstadoStock = estado
                };
                _ctrl.RegistrarTasacion(input);

                MessageBox.Show("Tasación registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // refrescamos todo
                CargarOfertas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al tasar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
