using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;


namespace AutoGestion.Vista
{
    public partial class ABMUsuarios : UserControl
    {
        public ABMUsuarios()
        {
            InitializeComponent();
            CargarUsuarios(); //LLena el DataGridView al iniciar la pantalla
            txtID.ReadOnly = true;
            txtClave.UseSystemPasswordChar = true;
        }

        private void CargarUsuarios()
        {
            var usuarios = UsuarioXmlService.Leer();
            dgvUsuarios.DataSource = usuarios.Select(u => new
            {
                u.ID,
                u.Nombre,
                Clave = u.Clave.DesencriptarPassword()
            }).ToList();

            dgvUsuarios.Columns["ID"].Width = 40;
            dgvUsuarios.Columns["Nombre"].Width = 150;
            dgvUsuarios.Columns["Clave"].Width = 150;
        }

        // el boton valida los datos ingresados y agrega un nuevo usuario al XML
        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string clave = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(clave))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            // Carga la lista actual de usuarios y verifica si el nombre ya existe
            var usuarios = UsuarioXmlService.Leer();
            if (usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El usuario ya existe.");
                return;
            }

            // Crea un nuevo usuario con un ID generado y la contraseña encriptada
            var nuevo = new Usuario
            {
                ID = GeneradorID.ObtenerID<Usuario>(),
                Nombre = nombre,
                Clave = Encriptacion.EncriptarPassword(clave),
                Rol = null
            };

            usuarios.Add(nuevo);
            UsuarioXmlService.Guardar(usuarios);
            MessageBox.Show("Usuario agregado correctamente.");
            CargarUsuarios();
            LimpiarCampos();
        }

        // El boton elimina el usuario seleccionado del XML
        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            var confirmar = MessageBox.Show($"¿Eliminar al usuario '{nombre}'?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirmar == DialogResult.Yes)
            {
                UsuarioXmlService.Eliminar(nombre);
                MessageBox.Show("Usuario eliminado.");
                CargarUsuarios();
                LimpiarCampos();
            }
        }

        // Alterna la visibilidad de la contraseña en el campo de texto
        private void chkVerClave_CheckedChanged_1(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = !chkVerClave.Checked;
        }

        private void dgvUsuarios_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow != null)
            {
                txtID.Text = dgvUsuarios.CurrentRow.Cells["ID"].Value.ToString();
                txtNombre.Text = dgvUsuarios.CurrentRow.Cells["Nombre"].Value.ToString();
                txtClave.Text = dgvUsuarios.CurrentRow.Cells["Clave"].Value.ToString();
            }
        }

        private void LimpiarCampos()
        {
            txtID.Clear();
            txtNombre.Clear();
            txtClave.Clear();
            chkVerClave.Checked = false;
        }

        // El boton modifica los datos del usuario seleccionado en el XML
        private void btnModificar_Click(object sender, EventArgs e)
        {
            // Valida ID seleccionado
            if (!int.TryParse(txtID.Text, out int id))
            {
                MessageBox.Show("Seleccione un usuario válido de la lista.");
                return;
            }

            string nuevoNombre = txtNombre.Text.Trim();
            string nuevaClave = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevaClave))
            {
                MessageBox.Show("Nombre y contraseña no pueden estar vacíos.");
                return;
            }

            var usuarios = UsuarioXmlService.Leer();

            var existente = usuarios.FirstOrDefault(u => u.ID == id);
            if (existente == null)
            {
                MessageBox.Show("No se encontró el usuario a modificar.");
                return;
            }

            existente.Nombre = nuevoNombre;
            existente.Clave = Encriptacion.EncriptarPassword(nuevaClave);

            UsuarioXmlService.Guardar(usuarios);
            MessageBox.Show("Usuario modificado correctamente.");
            CargarUsuarios();
            LimpiarCampos();
        }
    }
}
