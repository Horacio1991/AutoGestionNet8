using AutoGestion.DAO.Modelos;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;


namespace AutoGestion.Vista
{
    // Permite gestionar:
    //  - Plantillas de permisos (menús y submenús).
    //  - Roles (conjuntos de plantillas).
    //  - Asignación de roles a usuarios.
    // </summary>
    public partial class AsignarRoles : UserControl
    {
        private List<PermisoCompuesto> _plantillas;   // Árbol de menús prediseñados
        private List<PermisoCompuesto> _roles;        // Roles (agrupaciones de plantillas)
        private List<Usuario> _usuarios;    // Usuarios del sistema

        // Diccionario de menús a items (para creación de plantillas)
        private readonly Dictionary<string, List<string>> _menuItems = new()
        {
            ["Gestión Ventas"] = new() { "Solicitar Modelo", "Registrar Cliente", "Realizar Pago", "Autorizar Venta", "Emitir Factura", "Realizar Entrega" },
            ["Gestión Compras"] = new() { "Registrar Oferta", "Evaluar Vehículo", "Tasar Vehículo", "Registrar Compra" },
            ["Gestión Comisiones"] = new() { "Registrar Comisión", "Consultar Comisiones" },
            ["Gestión Turnos"] = new() { "Registrar Turno", "Registrar Asistencia" },
            ["Seguridad"] = new() { "Asignar Roles", "Dashboard", "Backup", "Restore", "Bitacora", "Cerrar Sesión" },
            ["Usuarios"] = new() { "ABM Usuarios" }
        };

        public AsignarRoles()
        {
            InitializeComponent();
            // Carga inicial de datos y configuración de eventos
            CargarDatosIniciales();
            WireUpEvents();
        }

        // Lee plantillas, roles y usuarios desde XML y las muestra en los TreeViews.
        private void CargarDatosIniciales()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML"));

                _plantillas = PermisoPlantillaXmlService.Leer();
                _roles = RolXmlService.Leer();
                _usuarios = UsuarioXmlService.Leer();

                CargarTreeViewPermisos();
                CargarTreeViewRoles();
                CargarTreeViewUsuarios();
                CargarComboPermisoMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos de permisos/roles/usuarios: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Asocia manejadores a los eventos de botones y controles.
        private void WireUpEvents()
        {
            // Plantillas
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            cmbPermisoMenu.SelectedIndexChanged += CmbPermisoMenu_SelectedIndexChanged;

            // Roles
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;
            btnAsociarPermisoARol.Click += BtnAsociarPermisoARol_Click;

            // Usuarios
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += BtnAsociarRolAUsuario_Click;
            btnQuitarRolUsuario.Click += BtnQuitarRolUsuario_Click;

            // Mostrar/ocultar contraseña
            chkEncriptar.CheckedChanged += ChkEncriptar_CheckedChanged;
        }

        #region Plantillas de Permisos

        // Llena el combo de "Menú" con las llaves del diccionario _menuItems.
        private void CargarComboPermisoMenu()
        {
            cmbPermisoMenu.Items.Clear();
            foreach (var menu in _menuItems.Keys)
                cmbPermisoMenu.Items.Add(menu);
        }
        private void CargarTreeViewPermisos()
        {
            tvPermisos.Nodes.Clear();
            foreach (var plantilla in _plantillas)
                tvPermisos.Nodes.Add(CrearNodoRecursivo(plantilla));
            tvPermisos.ExpandAll();
        }

        // Construye un TreeNode recursivamente a partir de un PermisoCompuesto
        // usado para mostrar la jerarquía de permisos en el TreeView.
        private TreeNode CrearNodoRecursivo(IPermiso permiso)
        {
            var node = new TreeNode(permiso.Nombre) { Tag = permiso };
            if (permiso is PermisoCompuesto pc)
                foreach (var hijo in pc.Hijos)
                    node.Nodes.Add(CrearNodoRecursivo(hijo));
            return node;
        }

        // Carga los menús disponibles en el combo para crear nuevas plantillas de permisos.
        private void CmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();
            if (cmbPermisoMenu.SelectedItem is string menu
                && _menuItems.TryGetValue(menu, out var items))
            {
                cmbPermisoItem.Items.AddRange(items.ToArray());
            }
        }

        // Da de alta una plantilla nueva o agrega sub-permisos a la seleccionada.
        // Por ejemplo, se crea "Vendedores" con submenús "Gestión Ventas", "Gestion Comisinoes", etc.,
        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var nombre = txtNombrePermiso.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingresá un nombre para la plantilla o permiso.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var seleccionado = tvPermisos.SelectedNode?.Tag as PermisoCompuesto;

                if (seleccionado == null)
                {
                    // Crear nueva plantilla raíz
                    if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                        throw new ApplicationException("Esa plantilla ya existe.");

                    var nueva = new PermisoCompuesto
                    {
                        ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                        Nombre = nombre
                    };
                    _plantillas.Add(nueva);
                }
                else
                {
                    // En un nodo existente: agregar submenú o ítem
                    if (cmbPermisoMenu.SelectedItem is not string menu)
                        throw new ApplicationException("Seleccioná un menú en el combo para agrupar.");

                    // Obtener o crear sub-rol para ese menú
                    var subCompuesto = seleccionado.HijosCompuestos
                        .FirstOrDefault(c => c.Nombre == menu)
                        ?? new PermisoCompuesto
                        {
                            ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                            Nombre = menu
                        }.Also(p => seleccionado.Agregar(p));

                    if (cmbPermisoItem.SelectedItem is string item)
                    {
                        // Agregar ítem simple al submenú
                        subCompuesto.Agregar(new PermisoSimple
                        {
                            ID = GeneradorID.ObtenerID<PermisoSimple>(),
                            Nombre = item
                        });
                    }
                }

                PermisoPlantillaXmlService.Guardar(_plantillas);
                CargarTreeViewPermisos();
                // no limpiamos txtNombrePermiso para facilitar múltiples altas
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear plantilla/permiso: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Gestión de Roles

        private void CargarTreeViewRoles()
        {
            tvRoles.Nodes.Clear();
            foreach (var rol in _roles)
                tvRoles.Nodes.Add(new TreeNode(rol.Nombre) { Tag = rol });
            tvRoles.ExpandAll();
        }

        private void BtnAltaRol_Click(object sender, EventArgs e)
        {
            var nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre de rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe ese rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var nuevo = new PermisoCompuesto
                {
                    ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                    Nombre = nombre
                };
                _roles.Add(nuevo);
                RolXmlService.Guardar(_roles);
                txtNombreRol.Clear();
                CargarTreeViewRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol para modificar.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nuevoNombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nuevoNombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                rol.Nombre = nuevoNombre;
                RolXmlService.Guardar(_roles);
                txtNombreRol.Clear();
                CargarTreeViewRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol para eliminar.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Eliminar el rol '{rol.Nombre}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _roles.RemoveAll(r => r.ID == rol.ID);
                RolXmlService.Guardar(_roles);
                txtNombreRol.Clear();
                CargarTreeViewRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompuesto rol)
            {
                txtNombreRol.Text = rol.Nombre;
                CargarTreeViewPermisosPorRol(rol);
            }
        }

        private void CargarTreeViewPermisosPorRol(PermisoCompuesto rol)
        {
            tvPermisosPorRol.Nodes.Clear();
            tvPermisosPorRol.Nodes.Add(CrearNodoRecursivo(rol));
            tvPermisosPorRol.ExpandAll();
        }

        private void BtnAsociarPermisoARol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tvPermisos.SelectedNode?.Tag is not PermisoCompuesto plantilla)
            {
                MessageBox.Show("Seleccioná una plantilla.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                rol.Agregar(plantilla);
                RolXmlService.Guardar(_roles);
                CargarTreeViewPermisosPorRol(rol);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al asociar plantilla al rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Gestión de Usuarios

        private void CargarTreeViewUsuarios()
        {
            tvUsuarios.Nodes.Clear();
            foreach (var u in _usuarios)
                tvUsuarios.Nodes.Add(new TreeNode(u.Nombre) { Tag = u });
            tvUsuarios.ExpandAll();
        }

        private void TvUsuarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is not Usuario usr) return;

            txtIdUsuario.Text = usr.ID.ToString();
            txtNombreUsuario.Text = usr.Nombre;
            txtContrasenaUsuario.Text = chkEncriptar.Checked
                ? Encriptacion.DesencriptarPassword(usr.Clave)
                : usr.Clave;

            // Mostrar sólo los permisos de ese usuario
            CargarTreeViewPermisosPorUsuario(usr);
        }

        private void CargarTreeViewPermisosPorUsuario(Usuario usr)
        {
            tvPermisosPorUsuario.Nodes.Clear();
            if (usr.Rol is PermisoCompuesto rol)
            {
                tvPermisosPorUsuario.Nodes.Add(CrearNodoRecursivo(rol));
                tvPermisosPorUsuario.ExpandAll();
            }
        }

        private void BtnAsociarRolAUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr)
            {
                MessageBox.Show("Seleccioná un usuario.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                usr.Rol = rol;
                UsuarioXmlService.Guardar(_usuarios);
                CargarTreeViewPermisosPorUsuario(usr);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al asignar rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuitarRolUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr)
            {
                MessageBox.Show("Seleccioná un usuario.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (usr.Rol == null)
            {
                MessageBox.Show("Ese usuario no tiene rol asignado.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"¿Quitar el rol '{usr.Rol.Nombre}' a '{usr.Nombre}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                usr.Rol = null;
                UsuarioXmlService.Guardar(_usuarios);
                CargarTreeViewPermisosPorUsuario(usr);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al quitar rol: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkEncriptar_CheckedChanged(object sender, EventArgs e)
        {
            // Alterna la visualización de la contraseña
            if (tvUsuarios.SelectedNode?.Tag is Usuario usr)
            {
                txtContrasenaUsuario.Text = chkEncriptar.Checked
                    ? Encriptacion.DesencriptarPassword(usr.Clave)
                    : usr.Clave;
            }
        }

        #endregion
    }

    // Helper para inicialización inline
    static class Extensions
    {
        public static T Also<T>(this T self, Action<T> act) { act(self); return self; }
    }
}
