using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoGestion.CTRL_Vista;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;

namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        // --------------------------------------------------
        // Campos y constantes
        // --------------------------------------------------
        private readonly AsignarRolesController _ctrl = new();

        // Menús predeterminados → ítems
        private static readonly Dictionary<string, string[]> _menuItems = new()
        {
            ["Gestión Ventas"] = new[] { "Solicitar Modelo", "Registrar Cliente", "Realizar Pago", "Autorizar Venta", "Emitir Factura", "Realizar Entrega" },
            ["Gestión Compras"] = new[] { "Registrar Oferta", "Evaluar Vehículo", "Tasar Vehículo", "Registrar Compra" },
            ["Gestión Comisiones"] = new[] { "Registrar Comisión", "Consultar Comisiones" },
            ["Gestión Turnos"] = new[] { "Registrar Turno", "Registrar Asistencia" },
            ["Seguridad"] = new[] { "Asignar Roles", "Dashboard", "Backup", "Restore", "Bitacora", "Cerrar Sesión" },
            ["Usuarios"] = new[] { "ABM Usuarios" }
        };

        // --------------------------------------------------
        // Constructor & setup inicial
        // --------------------------------------------------
        public AsignarRoles()
        {
            InitializeComponent();
            // Empiezo con combo deshabilitados
            cmbPermisoMenu.Enabled = false;
            cmbPermisoItem.Enabled = false;

            CargarTodo();
            AsignarEventos();
        }

        // --------------------------------------------------
        // Métodos de carga / refresco
        // --------------------------------------------------
        private void CargarTodo()
        {
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

            // Habilitar menú solo si hay plantillas
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

        // --------------------------------------------------
        // UI Helpers
        // --------------------------------------------------
        private TreeNode CrearNodoRecursivo(IPermiso permiso)
        {
            var node = new TreeNode(permiso.Nombre) { Tag = permiso };
            if (permiso is PermisoCompuesto pc)
                foreach (var hijo in pc.Hijos)
                    node.Nodes.Add(CrearNodoRecursivo(hijo));
            return node;
        }

        // --------------------------------------------------
        // Asociar eventos
        // --------------------------------------------------
        private void AsignarEventos()
        {
            // Permisos
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            btnEliminarPermiso.Click += BtnEliminarPermiso_Click;
            cmbPermisoMenu.SelectedIndexChanged += CmbPermisoMenu_SelectedIndexChanged;
            tvPermisos.AfterSelect += TvPermisos_AfterSelect;

            // Roles
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            btnAsociarPermisoARol.Click += BtnAsociarPermisoARol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;
            btnQuitarPermisoRol.Click += BtnQuitarPermisoRol_Click;


            // Usuarios
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += BtnAsociarRolAUsuario_Click;
            btnQuitarRolUsuario.Click += BtnQuitarRolUsuario_Click;
            chkEncriptar.CheckedChanged += ChkEncriptar_CheckedChanged;
        }

        // --------------------------------------------------
        // Manejo de PLANTILLAS
        // --------------------------------------------------
        private void CmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();
            if (cmbPermisoMenu.SelectedItem is string menu
             && _menuItems.TryGetValue(menu, out var items))
            {
                cmbPermisoItem.Items.AddRange(items);
                cmbPermisoItem.Enabled = true;      // habilito ítems tras elegir menú
            }
            else
            {
                cmbPermisoItem.Enabled = false;
            }
        }

        private void TvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Al seleccionar un permiso compuesto, habilito el combo de menús
            if (e.Node?.Tag is PermisoCompuesto)
                cmbPermisoMenu.Enabled = true;
        }

        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var nombre = txtNombrePermiso.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre para la plantilla o submenú.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guardar selecciones para restaurar
            var menuSel = cmbPermisoMenu.SelectedItem as string;
            var itemSel = cmbPermisoItem.SelectedItem as string;

            try
            {
                bool esNuevaRaiz = true;
                if (tvPermisos.SelectedNode?.Tag is PermisoCompuesto selPc)
                    esNuevaRaiz = !selPc.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase);

                if (esNuevaRaiz)
                {
                    // Crear plantilla raíz
                    _ctrl.CrearPlantilla(nombre);

                    // Recargar todo y seleccionar la nueva raíz
                    CargarTodo();
                    var nodoNuevo = tvPermisos.Nodes
                                              .Cast<TreeNode>()
                                              .First(n => ((PermisoCompuesto)n.Tag).Nombre == nombre);
                    tvPermisos.SelectedNode = nodoNuevo;
                    nodoNuevo.Expand();

                    // reset combos para siguiente paso
                    cmbPermisoMenu.SelectedIndex = -1;
                    cmbPermisoItem.Items.Clear();
                    cmbPermisoItem.Enabled = true;
                    return;
                }

                // Añadir ítem a plantilla existente
                if (tvPermisos.SelectedNode?.Tag is not PermisoCompuesto raiz)
                    throw new ApplicationException("Seleccione primero la plantilla donde agregar el ítem.");
                if (string.IsNullOrEmpty(menuSel))
                    throw new ApplicationException("Seleccione primero un Menú principal.");
                if (string.IsNullOrEmpty(itemSel))
                    throw new ApplicationException("Seleccione primero un Ítem de acción.");

                _ctrl.AgregarItemAPlantilla(raiz.ID, menuSel, itemSel);

                // Recargar solo plantillas y re-seleccionar
                CargarPlantillas();
                var nodoRaiz = tvPermisos.Nodes
                                         .Cast<TreeNode>()
                                         .First(n => ((PermisoCompuesto)n.Tag).ID == raiz.ID);
                tvPermisos.SelectedNode = nodoRaiz;
                nodoRaiz.Expand();

                // Restaurar selecciones de combo
                cmbPermisoMenu.SelectedItem = menuSel;
                cmbPermisoItem.SelectedItem = itemSel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo guardar plantilla:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // 1) Refrescar solo el árbol de plantillas
                CargarPlantillas();

                // 2) Si hay un rol seleccionado, refrescar su árbol de permisos
                if (tvRoles.SelectedNode?.Tag is PermisoCompuesto rol)
                    TvRoles_AfterSelect(tvRoles, new TreeViewEventArgs(tvRoles.SelectedNode));

                // 3) Si hay un usuario seleccionado, refrescar su árbol de permisos
                if (tvUsuarios.SelectedNode?.Tag is Usuario usr)
                    TvUsuarios_AfterSelect(tvUsuarios, new TreeViewEventArgs(tvUsuarios.SelectedNode));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar permiso:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // --------------------------------------------------
        // Manejo de ROLES
        // --------------------------------------------------
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
                // Guarda ID de usuario seleccionado (si lo hay)
                int? usuarioId = tvUsuarios.SelectedNode?.Tag is Usuario u ? u.ID : null;

                // Elimina el rol
                _ctrl.EliminarRol(rol.ID);

                // 1) Refresca listas principales
                CargarTodo();

                // 2) Limpia los árboles de permisos por rol y por usuario
                tvPermisosPorRol.Nodes.Clear();
                tvPermisosPorUsuario.Nodes.Clear();

                // 3) Restaura la selección de usuario (si corresponde) para refrescar su permiso
                if (usuarioId.HasValue)
                {
                    var nodoUsuario = tvUsuarios.Nodes
                        .Cast<TreeNode>()
                        .FirstOrDefault(n => ((Usuario)n.Tag).ID == usuarioId.Value);

                    if (nodoUsuario != null)
                    {
                        tvUsuarios.SelectedNode = nodoUsuario;
                        TvUsuarios_AfterSelect(tvUsuarios, new TreeViewEventArgs(nodoUsuario));
                    }
                }
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
             || tvPermisos.SelectedNode?.Tag is not PermisoCompuesto plantilla)
            {
                MessageBox.Show("Seleccione un rol y una plantilla.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ctrl.AsociarPlantillaARol(rol.ID, plantilla.ID);

                // 1) Refrescar árbol de roles y plantillas
                CargarRoles();
                CargarPlantillas();

                // 2) Reconstruir selección del rol para disparar AfterSelect
                var nodoRol = tvRoles.Nodes
                                     .Cast<TreeNode>()
                                     .First(n => ((PermisoCompuesto)n.Tag).ID == rol.ID);
                tvRoles.SelectedNode = nodoRol;
                TvRoles_AfterSelect(tvRoles, new TreeViewEventArgs(nodoRol));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo asociar permiso a rol:\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _ctrl.QuitarPlantillaDeRol(rol.ID, plantilla.ID);

                // 1) Refrescar roles y plantillas
                CargarRoles();
                CargarPlantillas();

                // 2) Reconstruir selección del rol para disparar AfterSelect
                var nodoRol = tvRoles.Nodes
                                     .Cast<TreeNode>()
                                     .First(n => ((PermisoCompuesto)n.Tag).ID == rol.ID);
                tvRoles.SelectedNode = nodoRol;
                TvRoles_AfterSelect(tvRoles, new TreeViewEventArgs(nodoRol));
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

        // --------------------------------------------------
        // Manejo de USUARIOS
        // --------------------------------------------------
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
                // Refrescar solo el tree de permisos por usuario:
                tvPermisosPorUsuario.Nodes.Clear();
                // ya no tiene rol, así que queda vacío
                txtIdUsuario.Clear();
                txtNombreUsuario.Clear();
                txtContrasenaUsuario.Clear();
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
    }
}
