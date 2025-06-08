using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.Vista
{
    public partial class EvaluarEstado : UserControl
    {
        private readonly OfertaBLL _ofertaBLL = new();
        private readonly EvaluacionBLL _evaluacionBLL = new();
        private List<OfertaCompra> _ofertasDisponibles;


        public EvaluarEstado()
        {
            InitializeComponent();
            dtpFiltroFecha.Value = DateTime.Today;
            CargarOfertas();
        }

        private void CargarOfertas()
        {
            _ofertasDisponibles = _ofertaBLL.ObtenerOfertasConInspeccion();

            var items = _ofertasDisponibles.Select(o => new OfertaComboItem { Oferta = o }).ToList();

            cmbOfertas.DataSource = null;
            cmbOfertas.DataSource = items;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedItem is not OfertaComboItem item)
            {
                MessageBox.Show("Seleccione una oferta.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMotor.Text) ||
                string.IsNullOrWhiteSpace(txtCarroceria.Text) ||
                string.IsNullOrWhiteSpace(txtInterior.Text) ||
                string.IsNullOrWhiteSpace(txtDocumentacion.Text))
            {
                MessageBox.Show("Complete todos los campos técnicos.");
                return;
            }

            var evaluacion = new EvaluacionTecnica
            {
                ID = GeneradorID.ObtenerID<EvaluacionTecnica>(),
                EstadoMotor = txtMotor.Text,
                EstadoCarroceria = txtCarroceria.Text,
                EstadoInterior = txtInterior.Text,
                EstadoDocumentacion = txtDocumentacion.Text,
                Observaciones = txtObservaciones.Text
            };

            _evaluacionBLL.GuardarEvaluacion(item.Oferta, evaluacion);
            MessageBox.Show("Evaluación registrada correctamente.");
            LimpiarFormulario();
            CargarOfertas();
        }

        private void FiltrarPorFecha()
        {
            DateTime seleccionada = dtpFiltroFecha.Value.Date;

            var ofertasFiltradas = _ofertasDisponibles
                .Where(o => o.FechaInspeccion.Date == seleccionada)
                .Select(o => new OfertaComboItem { Oferta = o })
                .ToList();

            cmbOfertas.DataSource = null;
            cmbOfertas.DataSource = ofertasFiltradas;

            if (ofertasFiltradas.Count == 0)
                MessageBox.Show("No hay ofertas para la fecha seleccionada.");
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
