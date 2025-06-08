using AutoGestion.Entidades;
using AutoGestion.BLL;


namespace AutoGestion.Vista
{
    public partial class RegistrarDatos : UserControl
    {
        private readonly OfertaBLL _ofertaBLL = new();
        private readonly EvaluacionBLL _evaluacionBLL = new();
        private readonly VehiculoBLL _vehiculoBLL = new();

        private OfertaCompra _ofertaSeleccionada;

        public RegistrarDatos()
        {
            InitializeComponent();
            cmbEstadoStock.Items.AddRange(new[] { "Disponible", "Requiere reacondicionamiento" });
        }

        private void btnBuscarOferta_Click(object sender, EventArgs e)
        {
            string dominio = txtDominio.Text.Trim();

            if (string.IsNullOrEmpty(dominio))
            {
                MessageBox.Show("Ingrese un dominio.");
                return;
            }

            var ofertasSinRegistrar = _ofertaBLL.ObtenerOfertasSinRegistrar();
            var oferta = ofertasSinRegistrar
                .FirstOrDefault(o => o.Vehiculo.Dominio.Equals(dominio, StringComparison.OrdinalIgnoreCase));

            if (oferta == null)
            {
                MessageBox.Show("No se encontró ninguna oferta pendiente para ese dominio.");
                LimpiarCampos();
                return;
            }

            var evaluacion = _evaluacionBLL.ObtenerEvaluacionAsociada(oferta);

            if (evaluacion == null)
            {
                MessageBox.Show("No se encontró evaluación técnica para la oferta.");
                LimpiarCampos();
                return;
            }

            _ofertaSeleccionada = oferta;

            txtEvaluacion.Text = $"Motor: {evaluacion.EstadoMotor}, Chasis: {evaluacion.EstadoInterior}, Documentación: {evaluacion.EstadoDocumentacion}";
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (_ofertaSeleccionada == null)
            {
                MessageBox.Show("Debe buscar una oferta primero.");
                return;
            }

            if (cmbEstadoStock.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el estado final del vehículo.");
                return;
            }

            // Actualiza el estado elegido
            string estadoElegido = cmbEstadoStock.SelectedItem.ToString();
            _vehiculoBLL.ActualizarEstadoVehiculo(_ofertaSeleccionada.Vehiculo, estadoElegido);

            // Si el estado es "Disponible", lo agregamos al stock
            if (estadoElegido == "Disponible")
            {
                _vehiculoBLL.AgregarVehiculoAlStock(_ofertaSeleccionada.Vehiculo);
            }

            MessageBox.Show("Vehículo registrado correctamente en el sistema.");
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtDominio.Clear();
            txtEvaluacion.Clear();
            cmbEstadoStock.SelectedIndex = -1;
            _ofertaSeleccionada = null;
        }
    }
}
