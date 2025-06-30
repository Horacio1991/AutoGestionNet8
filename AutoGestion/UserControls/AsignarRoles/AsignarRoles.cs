using AutoGestion.DAO.Modelos;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        private List<PermisoCompuesto> _plantillas;
        private List<PermisoCompuesto> _roles;
        private List<Usuario> _usuarios;

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
            CargarDatosIniciales();
            WireUpEvents();
        }

        private void CargarDatosIniciales()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
            Directory.CreateDirectory(dir);

            _plantillas = PermisoPlantillaXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();

            CargarTreeViewPermisos();
            CargarTreeViewRoles();
            CargarTreeViewUsuarios();
            CargarComboPermisoMenu();
        }

        private void WireUpEvents()
        {
            // Plantillas
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            tvPermisos.AfterSelect += TvPermisos_AfterSelect;
            cmbPermisoMenu.SelectedIndexChanged += CmbPermisoMenu_SelectedIndexChanged;

            // Roles CRUD + asociación
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;
            btnAsociarPermisoARol.Click += BtnAsociarPermisoARol_Click;

            // Usuarios + asociación de rol
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += btnAsociarRolAUsuario_Click_1;

            // Usuarios
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect;
            btnAsociarRolAUsuario.Click += btnAsociarRolAUsuario_Click_1;
            btnQuitarRolUsuario.Click += btnQuitarRolUsuario_Click;

            // Nuevo: checkbox
            chkEncriptar.CheckedChanged += ChkEncriptar_CheckedChanged;
        }

        #region Plantillas de Permisos compuestos

        private void CargarTreeViewPermisos()
        {
            tvPermisos.Nodes.Clear();
            foreach (var plc in _plantillas)
                tvPermisos.Nodes.Add(CrearNodoRecursivo(plc));
            tvPermisos.ExpandAll();
        }

        private TreeNode CrearNodoRecursivo(IPermiso permiso)
        {
            var node = new TreeNode(permiso.Nombre) { Tag = permiso };
            if (permiso is PermisoCompuesto pc)
                foreach (var hijo in pc.Hijos)
                    node.Nodes.Add(CrearNodoRecursivo(hijo));
            return node;
        }

        private void TvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is IPermiso p)
                txtNombrePermiso.Text = p.Nombre;
        }

        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var texto = txtNombrePermiso.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                MessageBox.Show("Ingresá un nombre.");
                return;
            }

            var sel = tvPermisos.SelectedNode;
            if (sel == null)
            {
                // Crear nueva plantilla
                if (_plantillas.Any(x => x.Nombre.Equals(texto, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Esa plantilla ya existe.");
                    return;
                }
                _plantillas.Add(new PermisoCompuesto { ID = GeneradorID.ObtenerID<PermisoCompuesto>(), Nombre = texto });
            }
            else if (sel.Tag is PermisoCompuesto pc)
            {
                // Debe haber menú seleccionado
                if (cmbPermisoMenu.SelectedItem is not string menuName)
                {
                    MessageBox.Show("Seleccioná un Menú en el combo.");
                    return;
                }
                // Obtener o crear submenú
                var sub = pc.Hijos
                            .OfType<PermisoCompuesto>()
                            .FirstOrDefault(m => m.Nombre == menuName)
                          ?? new PermisoCompuesto { ID = GeneradorID.ObtenerID<PermisoCompuesto>(), Nombre = menuName }
                             .Also(m => pc.Agregar(m));

                // Si hay ítem seleccionado, añadirlo
                if (cmbPermisoItem.SelectedItem is string itemName)
                {
                    if (!sub.Hijos.OfType<PermisoSimple>().Any(i => i.Nombre == itemName))
                        sub.Agregar(new PermisoSimple { ID = GeneradorID.ObtenerID<PermisoSimple>(), Nombre = itemName });
                }
            }
            else
            {
                MessageBox.Show("Seleccioná una plantilla o submenú.");
                return;
            }

            // Guardar y refrescar
            PermisoPlantillaXmlService.Guardar(_plantillas);
            CargarTreeViewPermisos();
            txtNombrePermiso.Clear();
        }

        #endregion

        #region ComboBoxes Menú → Ítems

        private void CargarComboPermisoMenu()
        {
            cmbPermisoMenu.Items.Clear();
            foreach (var m in _menuItems.Keys)
                cmbPermisoMenu.Items.Add(m);
        }

        private void CmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();
            if (cmbPermisoMenu.SelectedItem is string m && _menuItems.TryGetValue(m, out var items))
                foreach (var it in items)
                    cmbPermisoItem.Items.Add(it);
        }

        #endregion

        #region Roles (CRUD) y asociación de Plantillas

        private void CargarTreeViewRoles()
        {
            tvRoles.Nodes.Clear();
            foreach (var r in _roles)
                tvRoles.Nodes.Add(new TreeNode(r.Nombre) { Tag = r });
            tvRoles.ExpandAll();
        }

        private void BtnAltaRol_Click(object sender, EventArgs e)
        {
            var nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre de Rol.");
                return;
            }
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe ese Rol.");
                return;
            }
            _roles.Add(new PermisoCompuesto { ID = GeneradorID.ObtenerID<PermisoCompuesto>(), Nombre = nombre });
            RolXmlService.Guardar(_roles);
            txtNombreRol.Clear();
            CargarTreeViewRoles();
        }

        private void BtnModificarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un Rol para modificar.");
                return;
            }
            var nuevoNombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nuevoNombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.");
                return;
            }
            rol.Nombre = nuevoNombre;
            RolXmlService.Guardar(_roles);
            txtNombreRol.Clear();
            CargarTreeViewRoles();
        }

        private void BtnEliminarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un Rol para eliminar.");
                return;
            }
            if (MessageBox.Show($"¿Eliminar el Rol '{rol.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            _roles.RemoveAll(r => r.ID == rol.ID);
            RolXmlService.Guardar(_roles);
            txtNombreRol.Clear();
            CargarTreeViewRoles();
        }

        private void TvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompuesto rol)
                CargarTreeViewPermisosPorRol(rol);
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
                MessageBox.Show("Seleccioná un rol.");
                return;
            }
            if (tvPermisos.SelectedNode?.Tag is not PermisoCompuesto plantilla)
            {
                MessageBox.Show("Seleccioná una plantilla.");
                return;
            }
            if (rol.Hijos.OfType<PermisoCompuesto>().Any(p => p.ID == plantilla.ID))
            {
                MessageBox.Show("Ese rol ya tiene asociada esa plantilla.");
                return;
            }
            rol.Agregar(plantilla);
            RolXmlService.Guardar(_roles);
            CargarTreeViewPermisosPorRol(rol);
        }

        #endregion

        #region Usuarios y asociación de Rol

        private void CargarTreeViewUsuarios()
        {
            tvUsuarios.Nodes.Clear();
            foreach (var u in _usuarios)
                tvUsuarios.Nodes.Add(new TreeNode(u.Nombre) { Tag = u });
            tvUsuarios.ExpandAll();
        }

        private void TvUsuarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is Usuario usr)
            {
                // Cargar nombre
                txtNombreUsuario.Text = usr.Nombre;
                // Cargar ID
                txtIdUsuario.Text = usr.ID.ToString();
                // Cargar contraseña encriptada
                txtContrasenaUsuario.Text = usr.Clave;
                // Mostrar encriptada por defecto
                chkEncriptar.Checked = false;

                // Refrescar permisos
                CargarTreeViewPermisosPorUsuario(usr);
            }
        }

        // Maneja el CheckedChanged para mostrar/ocultar encriptación
        private void ChkEncriptar_CheckedChanged(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr)
                return;

            if (chkEncriptar.Checked)
            {
                // Muestro desencriptada
                txtContrasenaUsuario.Text = Encriptacion.DesencriptarPassword(usr.Clave);
            }
            else
            {
                // Vuelvo a la encriptada
                txtContrasenaUsuario.Text = usr.Clave;
            }
        }

        private void btnAsociarRolAUsuario_Click_1(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }
            usr.Rol = rol;
            UsuarioXmlService.Guardar(_usuarios);
            // Refrescar la vista de permisos por usuario
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

        #endregion

        private void btnQuitarRolUsuario_Click(object sender, EventArgs e)
        {
            // 1) Debe haber un usuario seleccionado
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usr)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }

            // 2) Verifico que tenga rol asignado
            if (usr.Rol == null)
            {
                MessageBox.Show("Ese usuario no tiene ningún rol asignado.");
                return;
            }

            // 3) Confirmación
            var nombreRol = (usr.Rol as PermisoCompuesto)?.Nombre ?? "(desconocido)";
            if (MessageBox.Show(
                    $"¿Quitar el rol '{nombreRol}' al usuario '{usr.Nombre}'?",
                    "Confirmar",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            // 4) Quito el rol y guardo en el XML
            usr.Rol = null;
            UsuarioXmlService.Guardar(_usuarios);

            // 5) Refresco la vista de permisos del usuario (se quedará vacía)
            CargarTreeViewPermisosPorUsuario(usr);

            MessageBox.Show($"Se ha quitado el rol al usuario {usr.Nombre}.");
        }
    }

    // Helper para inicialización inline
    static class Extensions
    {
        public static T Also<T>(this T self, Action<T> act) { act(self); return self; }
    }
}
