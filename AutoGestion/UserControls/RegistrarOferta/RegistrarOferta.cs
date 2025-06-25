using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class RegistrarOferta : UserControl
    {
        private readonly OfertaController _ctrl = new();
        private OferenteDto _oferente;

        public RegistrarOferta()
        {
            InitializeComponent();
        }

        private void btnBuscarOferente_Click_1(object sender, EventArgs e)
        {
            var dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI válido.");
                return;
            }

            _oferente = _ctrl.BuscarOferente(dni);
            if (_oferente != null)
            {
                txtNombre.Text = _oferente.Nombre;
                txtApellido.Text = _oferente.Apellido;
                txtContacto.Text = _oferente.Contacto;
                MessageBox.Show("Oferente encontrado.");
            }
            else
            {
                MessageBox.Show("Oferente no encontrado. Complete los datos para registrarlo.");
                txtNombre.Clear();
                txtApellido.Clear();
                txtContacto.Clear();
            }
        }



        private void btnGuardarOferta_Click(object sender, EventArgs e)
        {
            // -- Validaciones básicas --
            if (string.IsNullOrWhiteSpace(txtDni.Text)
             || string.IsNullOrWhiteSpace(txtNombre.Text)
             || string.IsNullOrWhiteSpace(txtApellido.Text)
             || string.IsNullOrWhiteSpace(txtContacto.Text)
             || string.IsNullOrWhiteSpace(txtMarca.Text)
             || string.IsNullOrWhiteSpace(txtModelo.Text)
             || string.IsNullOrWhiteSpace(txtColor.Text)
             || string.IsNullOrWhiteSpace(txtDominio.Text))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            // 1) Prepara los DTOs de entrada
            var dtoOferente = new OferenteDto
            {
                Dni = txtDni.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Contacto = txtContacto.Text.Trim()
            };

            var dtoVehiculo = new VehiculoDto
            {
                Marca = txtMarca.Text.Trim(),
                Modelo = txtModelo.Text.Trim(),
                Año = (int)numAnio.Value,
                Color = txtColor.Text.Trim(),
                Dominio = txtDominio.Text.Trim(),
                Km = (int)numKm.Value
            };

            var input = new OfertaInputDto
            {
                Oferente = dtoOferente,
                Vehiculo = dtoVehiculo,
                FechaInspeccion = dtpFechaInspeccion.Value
            };

            // 2) Llama al controller
            bool ok = _ctrl.RegistrarOferta(input);
            MessageBox.Show(ok
                ? "Oferta registrada correctamente."
                : "Ocurrió un error al registrar la oferta.");

            // 3) Limpia el formulario
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtContacto.Clear();
            txtMarca.Clear();
            txtModelo.Clear();
            txtColor.Clear();
            txtDominio.Clear();
            numAnio.Value = numAnio.Minimum;
            numKm.Value = 0;
        }



    }
}
