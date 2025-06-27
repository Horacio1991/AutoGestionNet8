using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.XmlServices;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.Vista
{
    public partial class ABMUsuarios : UserControl
    {
        public ABMUsuarios()
        {
            InitializeComponent();
            txtID.ReadOnly = true;
            txtClave.UseSystemPasswordChar = true;
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            var usuarios = UsuarioXmlService.Leer();
            dgvUsuarios.DataSource = usuarios.Select(u => new {
                u.ID,
                u.Nombre,
                Clave = Encriptacion.DesencriptarPassword(u.Clave)
            }).ToList();

            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // el boton valida los datos ingresados y agrega un nuevo usuario al XML
        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            var nombre = txtNombre.Text.Trim();
            var clave = txtClave.Text.Trim();

            if (nombre == "" || clave == "")
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            var lista = UsuarioXmlService.Leer();
            if (lista.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El usuario ya existe.");
                return;
            }

            var nuevo = new Usuario
            {
                ID = GeneradorID.ObtenerID<Usuario>(),
                Nombre = nombre,
                Clave = Encriptacion.EncriptarPassword(clave),
                Rol = null
            };

            lista.Add(nuevo);
            UsuarioXmlService.Guardar(lista);

            MessageBox.Show("Usuario agregado correctamente.");
            LimpiarCampos();
            CargarUsuarios();
        }

        // El boton elimina el usuario seleccionado del XML
        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out int id))
            {
                MessageBox.Show("Seleccione un usuario de la lista.");
                return;
            }

            var usuarios = UsuarioXmlService.Leer();
            var user = usuarios.FirstOrDefault(u => u.ID == id);
            if (user == null) return;

            if (MessageBox.Show($"¿Eliminar a '{user.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            UsuarioXmlService.Eliminar(user.Nombre);
            MessageBox.Show("Usuario eliminado.");
            LimpiarCampos();
            CargarUsuarios();
        }

        // Alterna la visibilidad de la contraseña en el campo de texto
        private void chkVerClave_CheckedChanged_1(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = !chkVerClave.Checked;
        }

        private void dgvUsuarios_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;
            txtID.Text = dgvUsuarios.CurrentRow.Cells["ID"].Value.ToString();
            txtNombre.Text = dgvUsuarios.CurrentRow.Cells["Nombre"].Value.ToString();
            txtClave.Text = dgvUsuarios.CurrentRow.Cells["Clave"].Value.ToString();
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
            if (!int.TryParse(txtID.Text, out int id))
            {
                MessageBox.Show("Seleccione un usuario válido.");
                return;
            }

            var nombre = txtNombre.Text.Trim();
            var clave = txtClave.Text.Trim();
            if (nombre == "" || clave == "")
            {
                MessageBox.Show("Nombre y contraseña no pueden estar vacíos.");
                return;
            }

            var usuarios = UsuarioXmlService.Leer();
            var existente = usuarios.FirstOrDefault(u => u.ID == id);
            if (existente == null) return;

            existente.Nombre = nombre;
            existente.Clave = Encriptacion.EncriptarPassword(clave);
            UsuarioXmlService.Guardar(usuarios);

            MessageBox.Show("Usuario modificado correctamente.");
            LimpiarCampos();
            CargarUsuarios();
        }
    }
}
