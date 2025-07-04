using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.XmlServices;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.Vista
{
    // Permite crear, editar y eliminar usuarios, y muestra su clave desencriptada para edición.
    public partial class ABMUsuarios : UserControl
    {
        public ABMUsuarios()
        {
            InitializeComponent();
            txtID.ReadOnly = true;             // El ID lo asigna automáticamente
            txtClave.UseSystemPasswordChar = true; // Oculta la clave por defecto
            CargarUsuarios();                  // Carga los usuarios al iniciar
        }

        // Carga todos los usuarios desde el XML y los muestra en el DataGridView.
        private void CargarUsuarios()
        {
            try
            {
                var usuarios = UsuarioXmlService.Leer();
                dgvUsuarios.DataSource = usuarios
                    .Select(u => new
                    {
                        u.ID,
                        u.Nombre,
                        Clave = Encriptacion.DesencriptarPassword(u.Clave)
                    })
                    .ToList();

                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvUsuarios.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar usuarios: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Agrega un nuevo usuario al XML.
        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string clave = txtClave.Text.Trim();

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(clave))
                {
                    MessageBox.Show("Complete todos los campos.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var lista = UsuarioXmlService.Leer();
                if (lista.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("El usuario ya existe.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear nuevo Usuario con ID automático y clave encriptada
                var nuevo = new Usuario
                {
                    ID = GeneradorID.ObtenerID<Usuario>(),
                    Nombre = nombre,
                    Clave = Encriptacion.EncriptarPassword(clave),
                    Rol = null
                };

                lista.Add(nuevo);
                UsuarioXmlService.Guardar(lista);

                MessageBox.Show("Usuario agregado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al agregar usuario: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Elimina el usuario seleccionado tras confirmación.
        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtID.Text, out int id))
                {
                    MessageBox.Show("Seleccione un usuario de la lista.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usuarios = UsuarioXmlService.Leer();
                var user = usuarios.FirstOrDefault(u => u.ID == id);
                if (user == null) return;

                if (MessageBox.Show($"¿Eliminar al usuario '{user.Nombre}'?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                UsuarioXmlService.Eliminar(user.Nombre);

                MessageBox.Show("Usuario eliminado.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al eliminar usuario: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Muestra/oculta la clave en el TextBox.
        private void chkVerClave_CheckedChanged_1(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = !chkVerClave.Checked;
        }

        // Al cambiar la selección en el grid, carga los datos en los campos.
        private void dgvUsuarios_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            txtID.Text = dgvUsuarios.CurrentRow.Cells["ID"].Value?.ToString();
            txtNombre.Text = dgvUsuarios.CurrentRow.Cells["Nombre"].Value?.ToString();
            txtClave.Text = dgvUsuarios.CurrentRow.Cells["Clave"].Value?.ToString();
        }

        // Modifica el usuario seleccionado (nombre y clave).
        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtID.Text, out int id))
                {
                    MessageBox.Show("Seleccione un usuario válido.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nombre = txtNombre.Text.Trim();
                string clave = txtClave.Text.Trim();
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(clave))
                {
                    MessageBox.Show("Nombre y contraseña no pueden estar vacíos.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usuarios = UsuarioXmlService.Leer();
                var existente = usuarios.FirstOrDefault(u => u.ID == id);
                if (existente == null) return;

                existente.Nombre = nombre;
                existente.Clave = Encriptacion.EncriptarPassword(clave);

                UsuarioXmlService.Guardar(usuarios);

                MessageBox.Show("Usuario modificado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al modificar usuario: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Limpia los campos del formulario.
        private void LimpiarCampos()
        {
            txtID.Clear();
            txtNombre.Clear();
            txtClave.Clear();
            chkVerClave.Checked = false;
        }
    }
}
