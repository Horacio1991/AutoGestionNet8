using AutoGestion.Servicios;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.XmlServices;


namespace AutoGestion.Vista
{
    public partial class FormLogin : Form
    {
        // Lista de usuarios cargados desde el XML al iniciar el formulario
        private List<Usuario> _usuarios = new();

        public FormLogin()
        {
            InitializeComponent();
            Load += FormLogin_Load;
        }

        // Al cargar el formulario, lee los usuarios del XML y verifica existencia.
        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                _usuarios = UsuarioXmlService.Leer();

                if (_usuarios.Count == 0)
                    MessageBox.Show(
                        "No hay usuarios cargados en el sistema.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Si falla la lectura del XML, no permitimos avanzar
                MessageBox.Show(
                    $"Error al cargar usuarios: {ex.Message}",
                    "Error crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
        }

        // Evento click del botón Ingresar: valida credenciales y abre Form1.
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombreUsuario = txtUsuario.Text.Trim();
                string claveIngresada = txtClave.Text.Trim();

                if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(claveIngresada))
                {
                    MessageBox.Show(
                        "Debe ingresar usuario y contraseña.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                // Encriptamos la contraseña ingresada para comparar con la guardada
                string claveEncriptada = Encriptacion.EncriptarPassword(claveIngresada);

                // Busca un usuario que coincida con el nombre y la clave encriptada
                var usuario = _usuarios
                    .FirstOrDefault(u =>
                        u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) &&
                        u.Clave == claveEncriptada);

                if (usuario == null)
                {
                    MessageBox.Show(
                        "Usuario o contraseña incorrectos.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Guardamos en sesión y abrimos la ventana principal
                Sesion.UsuarioActual = usuario;
                var formMain = new Form1(usuario);
                formMain.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al autenticar: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
