using AutoGestion.Servicios;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.XmlServices;

namespace AutoGestion.Vista
{
    public partial class FormLogin : Form
    {
        // Lista de usuarios cargados desde el XML
        private List<Usuario> _usuarios = new();

        public FormLogin()
        {
            InitializeComponent();
            Load += FormLogin_Load;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // Cargar usuarios desde el XML al iniciar el formulario
            _usuarios = UsuarioXmlService.Leer();

            // Si no hay usuarios, mostrar un mensaje
            if (_usuarios.Count == 0)
                MessageBox.Show("No hay usuarios cargados en el sistema.");
        }

        // El boton valida las credenciales ingresadas por el usuario
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombre = txtUsuario.Text.Trim();
            string claveIngresada = txtClave.Text.Trim();

            // Encriptar la clave ingresada antes de compararla
            string claveEncriptada = Encriptacion.EncriptarPassword(claveIngresada);

            // Busca un usuario que coincida con el nombre y la clave encriptada
            // Si no encuentra, devuelve null
            var usuario = _usuarios.FirstOrDefault(u =>
                u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                u.Clave == claveEncriptada);

            if (usuario == null)
            {
                MessageBox.Show("Usuario o contraseña incorrectos.");
                return;
            }

            // Guardar en sesión
            Sesion.UsuarioActual = usuario;

            // Abrir FormMain
            Form1 formMain = new Form1();
            formMain.Show();
            this.Hide();
        }
    }
}
