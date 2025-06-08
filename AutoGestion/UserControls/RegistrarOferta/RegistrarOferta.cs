using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.Vista
{
    public partial class RegistrarOferta : UserControl
    {
        private readonly OferenteBLL _oferenteBLL = new();
        private readonly OfertaBLL _ofertaBLL = new();
        private Oferente _oferenteActual = null;

        public RegistrarOferta()
        {
            InitializeComponent();
        }

        private void btnBuscarOferente_Click_1(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI válido.");
                return;
            }

            var oferente = _oferenteBLL.BuscarPorDni(dni);
            if (oferente != null)
            {
                txtNombre.Text = oferente.Nombre;
                txtApellido.Text = oferente.Apellido;
                txtContacto.Text = oferente.Contacto;
                MessageBox.Show("Oferente encontrado.");
            }
            else
            {
                MessageBox.Show("Oferente no encontrado. Complete los datos para registrarlo.");
            }
        }


        private void btnGuardarOferta_Click(object sender, EventArgs e)
        {
            try
            {
                // Buscar o crear oferente
                var oferente = _oferenteBLL.BuscarPorDni(txtDni.Text.Trim());
                if (oferente == null)
                {
                    oferente = new Oferente
                    {
                        ID = GeneradorID.ObtenerID<Oferente>(),
                        Dni = txtDni.Text.Trim(),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Contacto = txtContacto.Text.Trim()
                    };
                    _oferenteBLL.GuardarOferente(oferente);
                }

                // Leer todas las ofertas para generar ID único de Vehículo
                var ofertasAnteriores = _ofertaBLL.ObtenerTodas();
                int ultimoIdVehiculo = ofertasAnteriores.Select(o => o.Vehiculo?.ID ?? 0).DefaultIfEmpty(0).Max();

                // Crear vehículo
                var vehiculo = new Vehiculo
                {
                    ID = ultimoIdVehiculo + 1,
                    Marca = txtMarca.Text.Trim(),
                    Modelo = txtModelo.Text.Trim(),
                    Año = (int)numAnio.Value,
                    Color = txtColor.Text.Trim(),
                    Dominio = txtDominio.Text.Trim(),
                    Km = (int)numKm.Value,
                    Estado = "En evaluación"
                };

                // Crear oferta
                var oferta = new OfertaCompra
                {
                    ID = GeneradorID.ObtenerID<OfertaCompra>(),
                    Oferente = oferente,
                    Vehiculo = vehiculo,
                    FechaInspeccion = dtpFechaInspeccion.Value
                };

                _ofertaBLL.RegistrarOferta(oferta);
                MessageBox.Show("Oferta registrada correctamente.");

                // Limpiar campos
                foreach (Control c in this.Controls)
                {
                    if (c is TextBox tb) tb.Clear();
                }

                numAnio.Value = numAnio.Minimum;
                numKm.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la oferta: " + ex.Message);
            }
        }



    }
}
