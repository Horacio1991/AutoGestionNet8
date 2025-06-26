using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class EvaluarEstado : UserControl
    {
        private readonly EvaluacionController _ctrl = new();
        private List<OfertaListDto> _ofertas;


        public EvaluarEstado()
        {
            InitializeComponent();
            dtpFiltroFecha.Value = DateTime.Today;
            CargarOfertas();
        }

        private void CargarOfertas()
        {
            _ofertas = _ctrl.ObtenerOfertasParaEvaluar();
            cmbOfertas.DataSource = _ofertas
                .Select(o => new { o.ID, Texto = $"{o.VehiculoResumen} – {o.FechaInspeccion:dd/MM/yyyy}" })
                .ToList();
            cmbOfertas.DisplayMember = "Texto";
            cmbOfertas.ValueMember = "ID";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedValue is not int ofertaId)
            {
                MessageBox.Show("Seleccione una oferta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validación de campos técnicos
            if (string.IsNullOrWhiteSpace(txtMotor.Text)
             || string.IsNullOrWhiteSpace(txtCarroceria.Text)
             || string.IsNullOrWhiteSpace(txtInterior.Text)
             || string.IsNullOrWhiteSpace(txtDocumentacion.Text))
            {
                MessageBox.Show("Complete todos los campos técnicos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Creo el DTO de entrada
            var dto = new EvaluacionInputDto
            {
                OfertaID = ofertaId,
                EstadoMotor = txtMotor.Text.Trim(),
                EstadoCarroceria = txtCarroceria.Text.Trim(),
                EstadoInterior = txtInterior.Text.Trim(),
                EstadoDocumentacion = txtDocumentacion.Text.Trim(),
                Observaciones = txtObservaciones.Text.Trim()
            };

            try
            {
                _ctrl.RegistrarEvaluacion(dto);
                MessageBox.Show("Evaluación registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                CargarOfertas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar evaluación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void FiltrarPorFecha()
        {
            var fecha = dtpFiltroFecha.Value.Date;
            var filtradas = _ofertas
                .Where(o => o.FechaInspeccion.Date == fecha)
                .ToList();
            if (!filtradas.Any())
            {
                MessageBox.Show("No hay ofertas en esa fecha.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cmbOfertas.DataSource = filtradas
                .Select(o => new { o.ID, Texto = $"{o.VehiculoResumen} – {o.FechaInspeccion:dd/MM/yyyy}" })
                .ToList();
        }



        private void LimpiarFormulario()
        {
            txtMotor.Clear();
            txtCarroceria.Clear();
            txtInterior.Clear();
            txtDocumentacion.Clear();
            txtObservaciones.Clear();
            cmbOfertas.SelectedIndex = -1;
        }

        private void btnFiltrarFecha_Click(object sender, EventArgs e)
        {
            FiltrarPorFecha();
        }
    }
}
