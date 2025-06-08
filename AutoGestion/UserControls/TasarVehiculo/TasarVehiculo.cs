using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;

namespace AutoGestion.Vista
{
    public partial class TasarVehiculo : UserControl
    {
        private readonly OfertaBLL _ofertaBLL = new();
        private readonly EvaluacionBLL _evaluacionBLL = new();
        private readonly TasaBLL _tasaBLL = new();
        private readonly VehiculoBLL _vehiculoBLL = new();
        private List<OfertaCompra> _ofertasEvaluadas;

        public TasarVehiculo()
        {
            InitializeComponent();
            CargarOfertas();
            cmbEstadoStock.Items.AddRange(new[] { "Disponible", "Requiere reacondicionamiento" });
        }

        private void CargarOfertas()
        {
            _ofertasEvaluadas = _ofertaBLL.ObtenerOfertasConEvaluacion();

            // Creamos una lista de objetos con ToString personalizado
            var items = _ofertasEvaluadas
                .Select(o => new OfertaComboItem { Oferta = o })
                .ToList();

            cmbOfertas.DataSource = null;
            cmbOfertas.DataSource = items;
        }


        private void cmbOfertas_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedItem is not OfertaComboItem seleccionado) return;

            var oferta = seleccionado.Oferta;
            var evaluacion = _evaluacionBLL.ObtenerEvaluacionAsociada(oferta);

            if (evaluacion != null)
            {
                txtEvaluacion.Text =
                    $"Motor: {evaluacion.EstadoMotor}\r\n" +
                    $"Carrocería: {evaluacion.EstadoCarroceria}\r\n" +
                    $"Interior: {evaluacion.EstadoInterior}\r\n" +
                    $"Documentación: {evaluacion.EstadoDocumentacion}\r\n" +
                    $"Observaciones: {evaluacion.Observaciones}";

                var rango = _tasaBLL.CalcularRangoTasacion(
                    oferta.Vehiculo.Modelo,
                    evaluacion.EstadoMotor,
                    oferta.Vehiculo.Km
                );

                txtRango.Text = rango != null
                    ? $"Entre {rango.Min:C} y {rango.Max:C}"
                    : "Sin valores de referencia";
            }
            else
            {
                txtEvaluacion.Text = "Sin evaluación disponible.";
                txtRango.Text = "";
            }
        }


        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (cmbOfertas.SelectedItem is not OfertaComboItem seleccionado)
            {
                MessageBox.Show("Seleccione una oferta.");
                return;
            }

            var oferta = seleccionado.Oferta;

            if (!decimal.TryParse(txtValorFinal.Text, out decimal valorFinal))
            {
                MessageBox.Show("Ingrese un valor numérico válido para el valor final.");
                return;
            }

            if (cmbEstadoStock.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el estado del vehículo en stock.");
                return;
            }

            // Guardar tasación
            _tasaBLL.RegistrarTasacion(oferta, valorFinal);

            // Actualizar estado del vehículo
            string nuevoEstado = cmbEstadoStock.SelectedItem.ToString();
            _vehiculoBLL.ActualizarEstadoStock(oferta.Vehiculo, nuevoEstado);

            MessageBox.Show("Tasación registrada y estado actualizado correctamente.");

            LimpiarFormulario();

        }

        private void LimpiarFormulario()
        {
            txtEvaluacion.Clear();
            txtRango.Clear();
            txtValorFinal.Clear();
            cmbEstadoStock.SelectedIndex = -1;
            cmbOfertas.SelectedIndex = -1;
        }


    }
}
