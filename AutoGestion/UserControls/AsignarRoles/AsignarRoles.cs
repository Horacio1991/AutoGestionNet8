using AutoGestion.CTRL_Vista;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;

namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        private readonly AsignarRolesController _ctrl = new();
        private static readonly Dictionary<string, string[]> _menuItems = new()
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
            cmbPermisoMenu.Enabled = false;
            cmbPermisoItem.Enabled = false;
            CargarTodo();
            AsignarEventos();
        }

        #region Carga y Refresco
        private void CargarTodo()
        {
            _ctrl.RecargarTodo();
            CargarUsuarios();
            CargarRoles();
            CargarPlantillas();
            PrepararCombos();
        }

        private void CargarUsuarios()
        {
            tvUsuarios.Nodes.Clear();
            foreach (var u in _ctrl.GetUsuarios())
                tvUsuarios.Nodes.Add(new TreeNode(u.Nombre) { Tag = u });
            tvUsuarios.ExpandAll();
        }

        private void CargarRoles()
        {
            tvRoles.Nodes.Clear();
            foreach (var r in _ctrl.GetRoles())
                tvRoles.Nodes.Add(new TreeNode(r.Nombre) { Tag = r });
            tvRoles.ExpandAll();
        }

        private void CargarPlantillas()
        {
            tvPermisos.Nodes.Clear();
            foreach (var p in _ctrl.GetPlantillas())
                tvPermisos.Nodes.Add(CrearNodoRecursivo(p));
            tvPermisos.ExpandAll();

            bool hay = tvPermisos.Nodes.Count > 0;
            cmbPermisoMenu.Enabled = hay;
            cmbPermisoItem.Enabled = hay;
        }

        private void PrepararCombos()
        {
            cmbPermisoMenu.Items.Clear();
            cmbPermisoMenu.Items.AddRange(_menuItems.Keys.ToArray());
            cmbPermisoItem.Items.Clear();
        }
        #endregion

        #region UI Helpers
        private TreeNode CrearNodoRecursivo(IPermiso permiso)
        {
            var node = new TreeNode(permiso.Nombre) { Tag = permiso };
            if (permiso is PermisoCompuesto pc)
                foreach (var hijo in pc.Hijos)
                    node.Nodes.Add(CrearNodoRecursivo(hijo));
            return node;
        }
        #endregion

        #region Wiring de eventos
        private void AsignarEventos()
        {
            // --- Plantillas ---
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            btnEliminarPermiso.Click += BtnEliminarPermiso_Click;
            cmbPermisoMenu.SelectedIndexChanged += CmbPermisoMenu_SelectedIndexChanged;
            tvPermisos.AfterSelect += TvPermisos_AfterSelect;

            // --- Roles ---
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            btnAsociarPermisoARol.Click += BtnAsociarPermisoARol_Click;
            btnQuitarPermisoRol.Click += BtnQuitarPermisoRol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;

            // --- Usuarios ---
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += BtnAsociarRolAUsuario_Click;
            btnQuitarRolUsuario.Click += BtnQuitarRolUsuario_Click;
            chkEncriptar.CheckedChanged += ChkEncriptar_CheckedChanged;
        }
        #endregion

        #region Handlers de Plantillas
        private void CmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();
            if (cmbPermisoMenu.SelectedItem is string menu &&
                _menuItems.TryGetValue(menu, out var items))
            {
                cmbPermisoItem.Items.AddRange(items);
                cmbPermisoItem.Enabled = true;
            }
            else
            {
                cmbPermisoItem.Enabled = false;
            }
        }

        private void TvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompuesto)
                cmbPermisoMenu.Enabled = true;
        }

        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var nombre = txtNombrePermiso.Text.Trim();
            var menuSel = cmbPermisoMenu.SelectedItem as string;
            var itemSel = cmbPermisoItem.SelectedItem as string;

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre para la plantilla o submenú.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ¿Nueva raíz?
                bool esNueva = tvPermisos.SelectedNode?.Tag is not PermisoCompuesto selPc ||
                               !selPc.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase);

                if (esNueva)
                {
                    _ctrl.CrearPlantilla(nombre);
                    CargarTodo();

                    // Selección inmediata
                    var nodoNuevo = tvPermisos.Nodes
                       .Cast<TreeNode>()
                       .First(n => ((PermisoCompuesto)n.Tag).Nombre == nombre);
                    tvPermisos.SelectedNode = nodoNuevo;
                    nodoNuevo.Expand();

                    cmbPermisoMenu.SelectedIndex = -1;
                    cmbPermisoItem.Items.Clear();
                    return;
                }

                // Añadir ítem a plantilla existente
                if (tvPermisos.SelectedNode.Tag is not PermisoCompuesto raiz)
                    throw new ApplicationException("Seleccione primero la plantilla donde agregar el ítem.");
                if (string.IsNullOrEmpty(menuSel))
                    throw new ApplicationException("Seleccione un Menú principal.");
                if (string.IsNullOrEmpty(itemSel))
                    throw new ApplicationException("Seleccione un Ítem de acción.");

                _ctrl.AgregarItemAPlantilla(raiz.ID, menuSel, itemSel);

                // Refrescar sólo plantillas y conservar selección
                CargarPlantillas();
                var nodoRaiz = tvPermisos.Nodes
                   .Cast<TreeNode>()
                   .First(n => ((PermisoCompuesto)n.Tag).ID == raiz.ID);
                tvPermisos.SelectedNode = nodoRaiz;
                nodoRaiz.Expand();

                cmbPermisoMenu.SelectedItem = menuSel;
                cmbPermisoItem.SelectedItem = itemSel;
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
                MessageBox.Show("Seleccione un permiso o plantilla para eliminar.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.EliminarPlantilla(permiso.ID);

                // Refrescar árboles afectados
                CargarPlantillas();
                if (tvRoles.SelectedNode != null)
                    TvRoles_AfterSelect(tvRoles, new TreeViewEventArgs(tvRoles.SelectedNode));
                if (tvUsuarios.SelectedNode != null)
                    TvUsuarios_AfterSelect(tvUsuarios, new TreeViewEventArgs(tvUsuarios.SelectedNode));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar permiso:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Handlers de Roles
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
                MessageBox.Show($"No se pudo crear rol:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var nuevo = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nuevo))
            {
                MessageBox.Show("El nombre no puede estar vacío.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _ctrl.ModificarRol(rol.ID, nuevo);
                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo modificar rol:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Guardar ID de usuario para refrescar su vista luego
            int? usrId = (tvUsuarios.SelectedNode?.Tag as Usuario)?.ID;

            try
            {
                _ctrl.EliminarRol(rol.ID);

                // 1) Refrescar todo
                CargarTodo();

                // 2) Limpiar vistas de rol/usuario
                tvPermisosPorRol.Nodes.Clear();
                tvPermisosPorUsuario.Nodes.Clear();

                // 3) Re-seleccionar usuario si existía
                if (usrId.HasValue)
                {
                    var nodo = tvUsuarios.Nodes
                      .Cast<TreeNode>()
                      .FirstOrDefault(n => ((Usuario)n.Tag).ID == usrId.Value);
                    if (nodo != null)
                    {
                        tvUsuarios.SelectedNode = nodo;
                        TvUsuarios_AfterSelect(tvUsuarios, new TreeViewEventArgs(nodo));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar rol:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAsociarPermisoARol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol ||
                tvPermisos.SelectedNode?.Tag is not PermisoCompuesto plant)
            {
                MessageBox.Show("Seleccione un rol y una plantilla.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _ctrl.AsociarPlantillaARol(rol.ID, plant.ID);
                // Refrescar sólo árboles de rol y plantilla
                CargarRoles();
                CargarPlantillas();
                // Mantener selección de rol
                tvRoles.SelectedNode = tvRoles.Nodes
                  .Cast<TreeNode>()
                  .First(n => ((PermisoCompuesto)n.Tag).ID == rol.ID);
                TvRoles_AfterSelect(tvRoles, new TreeViewEventArgs(tvRoles.SelectedNode));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo asociar permiso a rol:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuitarPermisoRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccione primero un rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tvPermisosPorRol.SelectedNode?.Tag is not PermisoCompuesto plantilla)
            {
                MessageBox.Show("Seleccione la plantilla dentro del rol.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(
                    $"¿Quitar la plantilla '{plantilla.Nombre}' del rol '{rol.Nombre}'?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                _ctrl.QuitarPlantillaDeRol(rol.ID, plantilla.ID);

                // Refrescar todo y limpiar vistas de rol/usuario
                CargarTodo();
                tvPermisosPorRol.Nodes.Clear();
                tvPermisosPorUsuario.Nodes.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo quitar plantilla del rol:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        #endregion

        #region Handlers de Usuarios
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
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr ||
                tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
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
                MessageBox.Show($"No se pudo asignar rol al usuario:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                tvPermisosPorUsuario.Nodes.Clear();
                txtIdUsuario.Clear();
                txtNombreUsuario.Clear();
                txtContrasenaUsuario.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo quitar rol al usuario:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkEncriptar_CheckedChanged(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is Usuario usr)
                txtContrasenaUsuario.Text = chkEncriptar.Checked
                    ? Encriptacion.DesencriptarPassword(usr.Clave)
                    : usr.Clave;
        }
        #endregion
    }
}
