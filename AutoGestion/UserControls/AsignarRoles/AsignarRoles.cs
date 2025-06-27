using AutoGestion.DAO.Modelos;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;

namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        // Catálogo de permisos simples (acciones)
        private List<PermisoSimple> _permisos;
        // Lista de roles (compuestos de permisos)
        private List<PermisoCompuesto> _roles;
        // Lista de usuarios
        private List<Usuario> _usuarios;

        public AsignarRoles()
        {
            InitializeComponent();
            CargarDatosIniciales();
            WireUpEvents();
        }

        private void CargarDatosIniciales()
        {
            // 1. Leemos del XML
            _permisos = PermisoXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();

            // 2. Cargamos los TreeViews
            CargarTreeViewPermisos();
            CargarTreeViewRoles();
            CargarTreeViewUsuarios();  // <-- Nuevo
        }

        private void WireUpEvents()
        {
            // Botones Permisos
            btnAltaPermiso.Click += BtnAltaPermiso_Click;
            btnEliminarPermiso.Click += BtnEliminarPermiso_Click;
            tvPermisos.AfterSelect += TvPermisos_AfterSelect;

            // Botones Roles
            btnAltaRol.Click += BtnAltaRol_Click;
            btnModificarRol.Click += BtnModificarRol_Click;
            btnEliminarRol.Click += BtnEliminarRol_Click;
            tvRoles.AfterSelect += TvRoles_AfterSelect;

            // Evento para selección de usuarios
            tvUsuarios.AfterSelect += TvUsuarios_AfterSelect; 

            // Asociar permisos a rol
            btnAsociarPermisoRol.Click += BtnAsociarPermisoRol_Click;
            btnQuitarPermisoRol.Click += BtnQuitarPermisoRol_Click;
        }

        #region Permisos Simples

        private void CargarTreeViewPermisos()
        {
            tvPermisos.Nodes.Clear();
            foreach (var p in _permisos)
            {
                var nodo = new TreeNode(p.Nombre) { Tag = p };
                tvPermisos.Nodes.Add(nodo);
            }
            tvPermisos.ExpandAll();
        }

        private void TvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoSimple p)
                txtNombrePermiso.Text = p.Nombre;
        }

        private void BtnAltaPermiso_Click(object sender, EventArgs e)
        {
            var nombre = txtNombrePermiso.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre de permiso.");
                return;
            }
            if (_permisos.Any(x => x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe ese permiso.");
                return;
            }
            var nuevo = new PermisoSimple
            {
                ID = GeneradorID.ObtenerID<PermisoSimple>(),
                Nombre = nombre
            };
            _permisos.Add(nuevo);
            PermisoXmlService.Guardar(_permisos);
            txtNombrePermiso.Clear();
            CargarTreeViewPermisos();
        }

        private void BtnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (tvPermisos.SelectedNode?.Tag is not PermisoSimple p)
            {
                MessageBox.Show("Seleccioná un permiso para eliminar.");
                return;
            }
            if (MessageBox.Show($"¿Eliminar permiso '{p.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            _permisos.RemoveAll(x => x.ID == p.ID);
            PermisoXmlService.Guardar(_permisos);
            txtNombrePermiso.Clear();
            CargarTreeViewPermisos();
        }

        #endregion

        #region Roles (PermisoCompuesto)

        private void CargarTreeViewRoles()
        {
            tvRoles.Nodes.Clear();
            foreach (var r in _roles)
            {
                var nodo = new TreeNode(r.Nombre) { Tag = r };
                tvRoles.Nodes.Add(nodo);
            }
            tvRoles.ExpandAll();
        }

        private void TvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompuesto rol)
            {
                txtNombreRol.Text = rol.Nombre;
                CargarTreeViewPermisosPorRol(rol);
            }
        }

        private void BtnAltaRol_Click(object sender, EventArgs e)
        {
            var nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese un nombre de rol.");
                return;
            }
            if (_roles.Any(x => x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe ese rol.");
                return;
            }
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

        private void BtnModificarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol para modificar.");
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
                MessageBox.Show("Seleccioná un rol para eliminar.");
                return;
            }
            if (MessageBox.Show($"¿Eliminar el rol '{rol.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            _roles.RemoveAll(r => r.ID == rol.ID);
            RolXmlService.Guardar(_roles);
            txtNombreRol.Clear();
            CargarTreeViewRoles();
        }

        #endregion

        #region Usuarios  // 

        private void CargarTreeViewUsuarios()
        {
            tvUsuarios.Nodes.Clear();
            foreach (var u in _usuarios)
            {
                var nodo = new TreeNode(u.Nombre) { Tag = u };
                tvUsuarios.Nodes.Add(nodo);
            }
            tvUsuarios.ExpandAll();
        }

        private void TvUsuarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is Usuario usuario)
            {
                txtNombreUsuario.Text = usuario.Nombre;
                // Aquí podrías, por ejemplo, mostrar su Rol o permisos
            }
        }

        #endregion

        #region Asociación Permisos ↔ Rol

        private void CargarTreeViewPermisosPorRol(PermisoCompuesto rol)
        {
            tvPermisosPorRol.Nodes.Clear();
            var root = new TreeNode(rol.Nombre);
            foreach (var hijo in rol.ObtenerHijos())
                root.Nodes.Add(new TreeNode(hijo.Nombre));
            tvPermisosPorRol.Nodes.Add(root);
            tvPermisosPorRol.ExpandAll();
        }

        private void BtnAsociarPermisoRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }
            if (tvPermisos.SelectedNode?.Tag is not PermisoSimple permiso)
            {
                MessageBox.Show("Seleccioná un permiso.");
                return;
            }
            if (rol.ObtenerHijos().Any(p => p is PermisoSimple ps && ps.ID == permiso.ID))
            {
                MessageBox.Show("Ese permiso ya está asignado.");
                return;
            }
            rol.Agregar(permiso);
            RolXmlService.Guardar(_roles);
            CargarTreeViewPermisosPorRol(rol);
        }

        private void BtnQuitarPermisoRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rol)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }
            if (tvPermisosPorRol.SelectedNode == null || tvPermisosPorRol.SelectedNode.Parent == null)
            {
                MessageBox.Show("Seleccioná un permiso en la lista de permisos del rol.");
                return;
            }
            var nombrePermiso = tvPermisosPorRol.SelectedNode.Text;
            var candidato = rol.ObtenerHijos()
                               .OfType<PermisoSimple>()
                               .FirstOrDefault(p => p.Nombre == nombrePermiso);
            if (candidato == null)
            {
                MessageBox.Show("Permiso no encontrado en el rol.");
                return;
            }
            rol.Quitar(candidato);
            RolXmlService.Guardar(_roles);
            CargarTreeViewPermisosPorRol(rol);
        }

        #endregion
    }
}
