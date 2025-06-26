using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class RegistrarDatos : UserControl
    {
        private readonly RegistrarDatosController _ctrl = new();
        private OfertaRegistroDto _dto;

        public RegistrarDatos()
        {
            InitializeComponent();
            cmbEstadoStock.Items.AddRange(new[] { "Disponible", "Requiere reacondicionamiento" });
        }

        private void btnBuscarOferta_Click(object sender, EventArgs e)
        {
            var dominio = txtDominio.Text.Trim();
            if (string.IsNullOrEmpty(dominio))
            {
                MessageBox.Show("Ingrese un dominio.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _dto = _ctrl.ObtenerOfertaPorDominio(dominio);
            if (_dto == null)
            {
                MessageBox.Show("No se encontró oferta o evaluación para ese dominio.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                return;
            }

            txtEvaluacion.Text = _dto.EvaluacionTexto;
        }


        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (_dto == null)
            {
                MessageBox.Show("Busque primero una oferta válida.",
                                "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            if (cmbEstadoStock.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un estado de stock.",
                                "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var input = new RegistrarDatosInputDto
            {
                OfertaID = _dto.OfertaID,
                EstadoStock = cmbEstadoStock.SelectedItem.ToString()
            };

            try
            {
                _ctrl.RegistrarDatos(input);
                MessageBox.Show("Datos guardados correctamente.",
                                "Éxito", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}",
                                "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtDominio.Clear();
            txtEvaluacion.Clear();
            cmbEstadoStock.SelectedIndex = -1;
            _dto = null;
        }
    }
}
