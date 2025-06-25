using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.Vista
{
    public partial class RegistrarCliente : UserControl
    {
        private readonly ClienteController _controller = new();

        public RegistrarCliente()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                string dni = txtDni.Text.Trim();
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string contacto = txtContacto.Text.Trim();

                if (string.IsNullOrEmpty(dni) ||
                    string.IsNullOrEmpty(nombre) ||
                    string.IsNullOrEmpty(apellido) ||
                    string.IsNullOrEmpty(contacto))
                {
                    MessageBox.Show("Complete todos los campos antes de registrar.");
                    return;
                }

                // Llamamos al controller y recibimos un DTO
                ClienteDto nuevo = _controller.RegistrarCliente(dni, nombre, apellido, contacto);

                MessageBox.Show("Cliente registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar cliente: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscarDNI_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI válido.");
                return;
            }

            // Ahora trabajamos con el DTO
            ClienteDto existente = _controller.BuscarCliente(dni);
            if (existente != null)
            {
                MessageBox.Show("El cliente ya está registrado.", "Cliente encontrado");

                txtNombre.Text = existente.Nombre;
                txtApellido.Text = existente.Apellido;
                txtContacto.Text = existente.Contacto;

                btnRegistrar.Enabled = false;
            }
            else
            {
                MessageBox.Show("Cliente no encontrado. Puede registrarlo.");
                LimpiarFormulario(keepDni: true);
                btnRegistrar.Enabled = true;
            }
        }

        private void LimpiarFormulario(bool keepDni = false)
        {
            if (!keepDni) txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtContacto.Clear();
            btnRegistrar.Enabled = true;
        }
    }
}
