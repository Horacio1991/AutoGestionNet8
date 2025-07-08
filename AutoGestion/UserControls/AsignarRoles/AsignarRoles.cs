using AutoGestion.CTRL_Vista;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;


namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        private readonly AsignarRolesController _ctrl = new();

        // Menús predeterminados → ítems
        private readonly Dictionary<string, string[]> _menuItems = new()
        {
            ["Gestión Ventas"] = new[] { "Solicitar Modelo", "Registrar Cliente", "Realizar Pago", "Autorizar Venta", "Emitir Factura", "Realizar Entrega" },
            ["Gestión Compras"] = new[] { "Registrar Oferta", "Evaluar Vehículo", "Tasar Vehículo", "Registrar Compra" },
            ["Gestión Comisiones"] = new[] { "Registrar Comisión", "Consultar Comisiones" },
            ["Gestión Turnos"] = new[] { "Registrar Turno", "Registrar Asistencia" },
            ["Seguridad"] = new[] { "Asignar Roles", "Dashboard", "Backup", "Restore", "Bitacora", "Cerrar Sesión" },
            ["Usuarios"] = new[] { "ABM Usuarios" }
        };

        public AsignarRoles()
        {
            InitializeComponent();
            CargarTodo();
            AsignarEventos();
        }

        private void CargarTodo()
        {
            // Usuarios
            tvUsuarios.Nodes.Clear();
            foreach (var u in _ctrl.GetUsuarios())
                tvUsuarios.Nodes.Add(new TreeNode(u.Nombre) { Tag = u });
            tvUsuarios.ExpandAll();

            // Roles
            tvRoles.Nodes.Clear();
            foreach (var r in _ctrl.GetRoles())
                tvRoles.Nodes.Add(new TreeNode(r.Nombre) { Tag = r });
            tvRoles.ExpandAll();

            // Plantillas
            tvPermisos.Nodes.Clear();
            foreach (var p in _ctrl.GetPlantillas())
                tvPermisos.Nodes.Add(CrearNodoRecursivo(p));
            tvPermisos.ExpandAll();

            // ComboMenus
            cmbPermisoMenu.Items.Clear();
            cmbPermisoMenu.Items.AddRange(_menuItems.Keys.ToArray());
            cmbPermisoItem.Items.Clear();

            RefrescarPlantillas();
            tvPermisos.SelectedNode = null;
        }

        private void RefrescarPlantillas()
        {
            tvPermisos.Nodes.Clear();
            foreach (var p in _ctrl.GetPlantillas())
                tvPermisos.Nodes.Add(CrearNodoRecursivo(p));
            tvPermisos.ExpandAll();
        }

        private void AsignarEventos()
        {
            // Plantillas
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            btnEliminarPermiso.Click += BtnEliminarPermiso_Click;
            cmbPermisoMenu.SelectedIndexChanged += CmbPermisoMenu_SelectedIndexChanged;

            // Roles
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            btnAsociarPermisoARol.Click += BtnAsociarPermisoARol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;

            // Usuarios
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += BtnAsociarRolAUsuario_Click;
            btnQuitarRolUsuario.Click += BtnQuitarRolUsuario_Click;
            chkEncriptar.CheckedChanged += ChkEncriptar_CheckedChanged;
        }

        private TreeNode CrearNodoRecursivo(IPermiso permiso)
        {
            var node = new TreeNode(permiso.Nombre) { Tag = permiso };
            if (permiso is PermisoCompuesto pc)
                foreach (var hijo in pc.Hijos)
                    node.Nodes.Add(CrearNodoRecursivo(hijo));
            return node;
        }

        // --- Plantillas ---
        private void CmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();
            if (cmbPermisoMenu.SelectedItem is string menu
             && _menuItems.TryGetValue(menu, out var items))
                cmbPermisoItem.Items.AddRange(items);
        }

        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var nombre = txtNombrePermiso.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre para la plantilla o submenú.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guardamos la selección actual de menú e ítem
            var menuSeleccionado = cmbPermisoMenu.SelectedItem as string;
            var itemSeleccionado = cmbPermisoItem.SelectedItem as string;

            try
            {
                // ¿Estamos creando una plantilla raíz nueva?
                bool esNuevaRaiz = true;
                if (tvPermisos.SelectedNode?.Tag is PermisoCompuesto selPc)
                {
                    esNuevaRaiz = !selPc.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase);
                }

                if (esNuevaRaiz)
                {
                    // 1) Crear plantilla raíz
                    _ctrl.CrearPlantilla(nombre);

                    // 2) Recargar TODO y seleccionar esa nueva raíz
                    CargarTodo();
                    var nodoNuevo = tvPermisos.Nodes
                                              .Cast<TreeNode>()
                                              .First(n => ((PermisoCompuesto)n.Tag).Nombre == nombre);
                    tvPermisos.SelectedNode = nodoNuevo;
                    nodoNuevo.Expand();

                    // 3) Reiniciar combos para que el usuario elija submenú
                    cmbPermisoMenu.SelectedIndex = -1;
                    cmbPermisoItem.Items.Clear();
                    return;
                }

                // --- Estamos añadiendo un ítem dentro de una plantilla existente ---
                if (tvPermisos.SelectedNode?.Tag is not PermisoCompuesto raiz)
                    throw new ApplicationException("Seleccione primero la plantilla donde agregar el ítem.");

                if (string.IsNullOrEmpty(menuSeleccionado))
                    throw new ApplicationException("Seleccione primero un Menú principal.");
                if (string.IsNullOrEmpty(itemSeleccionado))
                    throw new ApplicationException("Seleccione primero un Ítem de acción.");

                // 1) Llamada al controller para agregar submenú + ítem
                _ctrl.AgregarItemAPlantilla(raiz.ID, menuSeleccionado, itemSeleccionado);

                // 2) Recargar sólo el árbol de plantillas y reseleccionar la misma raíz
                RefrescarPlantillas();
                var nodoRaiz = tvPermisos.Nodes
                                         .Cast<TreeNode>()
                                         .First(n => ((PermisoCompuesto)n.Tag).ID == raiz.ID);
                tvPermisos.SelectedNode = nodoRaiz;
                nodoRaiz.Expand();

                // 3) RESTAURAR la selección de combos para que no pierdas el menú/ítem
                if (menuSeleccionado != null && cmbPermisoMenu.Items.Contains(menuSeleccionado))
                    cmbPermisoMenu.SelectedItem = menuSeleccionado;
                if (itemSeleccionado != null && cmbPermisoItem.Items.Contains(itemSeleccionado))
                    cmbPermisoItem.SelectedItem = itemSeleccionado;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo guardar plantilla:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (tvPermisos.SelectedNode?.Tag is not IPermiso permiso)
            {
                MessageBox.Show("Seleccione un permiso o plantilla para eliminar.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.EliminarPlantilla(permiso.ID);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar permiso:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Roles ---
        private void BtnAltaRol_Click(object sender, EventArgs e)
        {
            var nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre de rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.CrearRol(nombre);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo crear rol:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccione un rol para modificar.", "Validación",
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
                _ctrl.ModificarRol(rol.ID, nuevoNombre);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo modificar rol:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccione un rol para eliminar.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.EliminarRol(rol.ID);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar rol:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAsociarPermisoARol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol
             || tvPermisos.SelectedNode?.Tag is not PermisoCompuesto plant)
            {
                MessageBox.Show("Seleccione un rol y una plantilla.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.AsociarPlantillaARol(rol.ID, plant.ID);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo asociar permiso a rol:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is PermisoCompuesto rol)
            {
                txtNombreRol.Text = rol.Nombre;
                tvPermisosPorRol.Nodes.Clear();
                tvPermisosPorRol.Nodes.Add(CrearNodoRecursivo(rol));
                tvPermisosPorRol.ExpandAll();
            }
        }

        // --- Usuarios ---
        private void TvUsuarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Usuario usr)
            {
                txtIdUsuario.Text = usr.ID.ToString();
                txtNombreUsuario.Text = usr.Nombre;
                txtContrasenaUsuario.Text = chkEncriptar.Checked
                    ? Encriptacion.DesencriptarPassword(usr.Clave)
                    : usr.Clave;

                tvPermisosPorUsuario.Nodes.Clear();
                if (usr.Rol is PermisoCompuesto r)
                {
                    tvPermisosPorUsuario.Nodes.Add(CrearNodoRecursivo(r));
                    tvPermisosPorUsuario.ExpandAll();
                }
            }
        }

        private void BtnAsociarRolAUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr
             || tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccione usuario y rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.AsignarRolAUsuario(usr.ID, rol.ID);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo asignar rol al usuario:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuitarRolUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr || usr.Rol == null)
            {
                MessageBox.Show("Seleccione un usuario con rol asignado.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.QuitarRolDeUsuario(usr.ID);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo quitar rol al usuario:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkEncriptar_CheckedChanged(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is Usuario usr)
            {
                txtContrasenaUsuario.Text = chkEncriptar.Checked
                    ? Encriptacion.DesencriptarPassword(usr.Clave)
                    : usr.Clave;
            }
        }

        private void tvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Si selecciono un nodo y es un PermisoCompuesto, cargo su nombre en el TextBox
            if (e.Node?.Tag is PermisoCompuesto pc)
            {
                txtNombrePermiso.Text = pc.Nombre;
                // Limpio el combo para distinguir modo "crear raíz" vs "añadir a raíz"
                cmbPermisoMenu.SelectedIndex = -1;
                cmbPermisoItem.Items.Clear();
            }
        }
    }
}
