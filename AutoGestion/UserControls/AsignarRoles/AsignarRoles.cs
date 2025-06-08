using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Encriptacion;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;

namespace AutoGestion.Vista
{
    public partial class AsignarRoles : UserControl
    {
        private List<PermisoCompleto> _permisos = new();
        private List<PermisoCompuesto> _roles = new(); // lista de roles


        public AsignarRoles()
        {
            InitializeComponent();
            CargarCombos();
            _permisos = PermisoCompletoXmlService.Leer();
            CargarTreeViewPermisos();
            CargarTreeViewUsuarios();
            CargarTreeViewRoles();

        }

        private void CargarCombos()
        {
            cmbPermisoMenu.Items.Clear();
            cmbPermisoMenu.Items.AddRange(new[]
            {
                 "Gestión Ventas",
                 "Gestión Compras",
                 "Gestión Comisiones",
                 "Gestión Turnos",
                 "Seguridad"
            });

            cmbPermisoMenu.SelectedIndexChanged += cmbPermisoMenu_SelectedIndexChanged;
        }

        private void cmbPermisoMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPermisoItem.Items.Clear();

            switch (cmbPermisoMenu.SelectedItem.ToString())
            {
                case "Gestión Ventas":
                    cmbPermisoItem.Items.AddRange(new[]
                    {
                "Solicitar Modelo",
                "Registrar Cliente",
                "Realizar Pago",
                "Autorizar Venta",
                "Emitir Factura",
                "Realizar Entrega"
            });
                    break;

                case "Gestión Compras":
                    cmbPermisoItem.Items.AddRange(new[]
                    {
                "Registrar Oferta",
                "Evaluar Vehículo",
                "Tasar Vehículo",
                "Registrar Compra"
            });
                    break;

                case "Gestión Comisiones":
                    cmbPermisoItem.Items.AddRange(new[]
                    {
                "Registrar Comisión",
                "Consultar Comisiones"
            });
                    break;

                case "Gestión Turnos":
                    cmbPermisoItem.Items.AddRange(new[]
                    {
                "Registrar Turno",
                "Registrar Asistencia"
            });
                    break;

                case "Seguridad":
                    cmbPermisoItem.Items.AddRange(new[]
                    {
                "Asignar Roles",
                "Cerrar Sesión"
            });
                    break;
            }
        }





        private void btnAltaPermiso_Click(object sender, EventArgs e)
        {
            string nombrePermiso = txtNombrePermiso.Text.Trim();
            string menuSeleccionado = cmbPermisoMenu.SelectedItem?.ToString();
            string itemSeleccionado = cmbPermisoItem.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(nombrePermiso) || string.IsNullOrEmpty(menuSeleccionado) || string.IsNullOrEmpty(itemSeleccionado))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            var permisoExistente = _permisos.FirstOrDefault(p => p.Nombre == nombrePermiso);

            if (permisoExistente != null)
            {
                var menuExistente = permisoExistente.MenuItems.FirstOrDefault(m => m.Menu == menuSeleccionado);
                if (menuExistente != null)
                {
                    if (!menuExistente.Items.Contains(itemSeleccionado))
                    {
                        menuExistente.Items.Add(itemSeleccionado);
                    }
                    else
                    {
                        MessageBox.Show("Este ítem ya está asignado.");
                        return;
                    }
                }
                else
                {
                    permisoExistente.MenuItems.Add(new MenuPermiso
                    {
                        Menu = menuSeleccionado,
                        Items = new List<string> { itemSeleccionado }
                    });
                }
            }
            else
            {
                var nuevoPermiso = new PermisoCompleto
                {
                    ID = GeneradorID.ObtenerID<PermisoCompleto>(),
                    Nombre = nombrePermiso,
                    MenuItems = new List<MenuPermiso>
                    {
                        new MenuPermiso
                        {
                            Menu = menuSeleccionado,
                            Items = new List<string> { itemSeleccionado }
                        }
                    }
                };

                _permisos.Add(nuevoPermiso);
            }

            PermisoCompletoXmlService.Guardar(_permisos);
            CargarTreeViewPermisos();
        }

        private void CargarTreeViewPermisos()
        {
            tvPermisos.Nodes.Clear();

            foreach (var permiso in _permisos)
            {
                TreeNode permisoNode = new TreeNode(permiso.Nombre)
                {
                    Tag = permiso // ✅ ASIGNAMOS EL OBJETO AL NODO
                };

                foreach (var menu in permiso.MenuItems)
                {
                    TreeNode menuNode = new TreeNode(menu.Menu);
                    foreach (var item in menu.Items)
                    {
                        menuNode.Nodes.Add(new TreeNode(item));
                    }
                    permisoNode.Nodes.Add(menuNode);
                }

                tvPermisos.Nodes.Add(permisoNode);
            }
        }


        private void tvPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompleto permiso)
            {
                txtNombrePermiso.Text = permiso.Nombre;
            }
        }


        private void btnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (tvPermisos.SelectedNode == null || tvPermisos.SelectedNode.Tag is not PermisoCompleto permisoSeleccionado)
            {
                MessageBox.Show("Seleccioná un permiso válido para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show($"¿Seguro que deseas eliminar el permiso '{permisoSeleccionado.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirmar != DialogResult.Yes) return;

            _permisos.RemoveAll(p => p.ID == permisoSeleccionado.ID);
            PermisoCompletoXmlService.Guardar(_permisos);
            CargarTreeViewPermisos();
            txtNombrePermiso.Clear();
            MessageBox.Show("Permiso eliminado correctamente.");
        }

        private void CargarTreeViewUsuarios()
        {
            tvUsuarios.Nodes.Clear();
            var usuarios = UsuarioXmlService.Leer();

            foreach (var usuario in usuarios)
            {
                TreeNode nodo = new TreeNode(usuario.Nombre)
                {
                    Tag = usuario
                };
                tvUsuarios.Nodes.Add(nodo);
            }
        }

        private void tvUsuarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is not Usuario usuario) return;

            txtIdUsuario.Text = usuario.ID.ToString();
            txtNombreUsuario.Text = usuario.Nombre;
            txtContrasenaUsuario.Text = usuario.Clave; // Mostramos encriptada por defecto
            chkEncriptar.Checked = false;
            CargarTreeViewRolesPermisosDelUsuario(usuario); // ✅
        }

        private void chkEncriptar_CheckedChanged(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usuario) return;

            if (chkEncriptar.Checked)
                txtContrasenaUsuario.Text = Encriptacion.DesencriptarPassword(usuario.Clave);
            else
                txtContrasenaUsuario.Text = usuario.Clave;
        }

        private void CargarTreeViewRoles()
        {
            tvRoles.Nodes.Clear();
            _roles = RolXmlService.Leer();

            foreach (var rol in _roles)
            {
                TreeNode nodoRol = new TreeNode(rol.Nombre)
                {
                    Tag = rol
                };
                tvRoles.Nodes.Add(nodoRol);
            }
        }

        private void btnAltaRol_Click(object sender, EventArgs e)
        {
            string nombreRol = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombreRol))
            {
                MessageBox.Show("Ingresá un nombre para el rol.");
                return;
            }

            var nuevoRol = new PermisoCompuesto
            {
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = nombreRol
            };

            _roles.Add(nuevoRol);
            RolXmlService.Guardar(_roles);
            CargarTreeViewRoles();
            txtNombreRol.Clear();
        }

        private void tvRoles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is PermisoCompuesto rol)
            {
                txtNombreRol.Text = rol.Nombre;
                CargarTreeViewPermisosPorRol(rol); // ✅
            }
        }


        private void btnModificarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rolSeleccionado)
            {
                MessageBox.Show("Seleccioná un rol para modificar.");
                return;
            }

            string nuevoNombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nuevoNombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.");
                return;
            }

            rolSeleccionado.Nombre = nuevoNombre;
            RolXmlService.Guardar(_roles);
            CargarTreeViewRoles();
            txtNombreRol.Clear();
        }

        private void btnEliminarRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rolSeleccionado)
            {
                MessageBox.Show("Seleccioná un rol para eliminar.");
                return;
            }

            var confirmar = MessageBox.Show($"¿Eliminar el rol '{rolSeleccionado.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirmar != DialogResult.Yes) return;

            _roles.RemoveAll(r => r.ID == rolSeleccionado.ID);
            RolXmlService.Guardar(_roles);
            CargarTreeViewRoles();
            txtNombreRol.Clear();
        }

        private void CargarTreeViewRolesPermisosDelUsuario(Usuario usuario)
        {
            tvRolesPermisosUsuario.Nodes.Clear();

            TreeNode nodoUsuario = new TreeNode(usuario.Nombre);

            if (usuario.Rol != null)
            {
                TreeNode nodoRol = new TreeNode(usuario.Rol.Nombre);

                foreach (var permiso in usuario.Rol.ObtenerHijos())
                {
                    if (permiso is PermisoCompleto pc)
                    {
                        TreeNode nodoPermiso = new TreeNode(pc.Nombre);
                        foreach (var menu in pc.MenuItems)
                        {
                            TreeNode nodoMenu = new TreeNode(menu.Menu);
                            foreach (var item in menu.Items)
                                nodoMenu.Nodes.Add(new TreeNode(item));
                            nodoPermiso.Nodes.Add(nodoMenu);
                        }
                        nodoRol.Nodes.Add(nodoPermiso);
                    }
                    else
                    {
                        nodoRol.Nodes.Add(new TreeNode(permiso.Nombre));
                    }
                }

                nodoUsuario.Nodes.Add(nodoRol);
            }

            tvRolesPermisosUsuario.Nodes.Add(nodoUsuario);
            tvRolesPermisosUsuario.ExpandAll();
        }

        private void btnAsociarRolUsuario_Click(object sender, EventArgs e)
        {
            // Validación: selección de usuario
            if (tvUsuarios.SelectedNode == null || tvUsuarios.SelectedNode.Tag is not Usuario usuarioSeleccionado)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }

            // Validación: selección de rol
            if (tvRoles.SelectedNode == null || tvRoles.SelectedNode.Tag is not PermisoCompuesto rolSeleccionado)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }

            // Asociar el rol al usuario
            usuarioSeleccionado.Rol = rolSeleccionado;

            // Guardar en XML
            var usuarios = UsuarioXmlService.Leer();
            var usuario = usuarios.FirstOrDefault(u => u.ID == usuarioSeleccionado.ID);
            if (usuario != null)
            {
                usuario.Rol = rolSeleccionado;
                UsuarioXmlService.Guardar(usuarios);
                MessageBox.Show("Rol asignado correctamente.");
            }

            CargarTreeViewRolesPermisosUsuario(usuarioSeleccionado);

        }

        private void CargarTreeViewRolesPermisosUsuario(Usuario usuario)
        {
            tvRolesPermisosUsuario.Nodes.Clear();

            TreeNode usuarioNode = new TreeNode(usuario.Nombre);

            if (usuario.Rol is PermisoCompuesto rol)
            {
                TreeNode rolNode = new TreeNode("Rol: " + rol.Nombre);

                foreach (var permiso in rol.ObtenerHijos())
                {
                    if (permiso is PermisoCompleto pc)
                    {
                        TreeNode nodoPermiso = new TreeNode(pc.Nombre);
                        foreach (var menu in pc.MenuItems)
                        {
                            TreeNode nodoMenu = new TreeNode(menu.Menu);
                            foreach (var item in menu.Items)
                                nodoMenu.Nodes.Add(new TreeNode(item));
                            nodoPermiso.Nodes.Add(nodoMenu);
                        }
                        rolNode.Nodes.Add(nodoPermiso);
                    }
                    else
                    {
                        rolNode.Nodes.Add(new TreeNode(permiso.Nombre));
                    }

                }

                usuarioNode.Nodes.Add(rolNode);
            }

            tvRolesPermisosUsuario.Nodes.Add(usuarioNode);
            tvRolesPermisosUsuario.ExpandAll();
        }

        private void btnQuitarRolUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode == null || tvUsuarios.SelectedNode.Tag is not Usuario usuarioSeleccionado)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }

            var confirmar = MessageBox.Show("¿Estás seguro que querés quitar el rol de este usuario?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirmar != DialogResult.Yes) return;

            // Leer desde XML para tener la lista actualizada
            var usuarios = UsuarioXmlService.Leer();
            var usuario = usuarios.FirstOrDefault(u => u.ID == usuarioSeleccionado.ID);
            if (usuario != null)
            {
                usuario.Rol = null;
                UsuarioXmlService.Guardar(usuarios);
                MessageBox.Show("Rol quitado correctamente.");

                // Actualizar en memoria también
                usuarioSeleccionado.Rol = null;

                // ✅ Volver a cargar el TreeView con usuario sin rol
                CargarTreeViewRolesPermisosUsuario(usuarioSeleccionado);
            }
        }

        private void btnAsociarPermisoUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usuarioSeleccionado)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }

            if (tvPermisos.SelectedNode?.Tag is not PermisoCompleto permisoSeleccionado)
            {
                MessageBox.Show("Seleccioná un permiso.");
                return;
            }

            // Asegurar que el usuario tenga un rol asignado (como contenedor)
            if (usuarioSeleccionado.Rol == null)
                usuarioSeleccionado.Rol = new PermisoCompuesto { Nombre = "Permisos Individuales" };

            // Evitar duplicados
            if (!usuarioSeleccionado.Rol.ObtenerHijos().Any(p => p is PermisoCompleto pc && pc.Nombre == permisoSeleccionado.Nombre))
            {
                usuarioSeleccionado.Rol.Agregar(permisoSeleccionado);
            }
            else
            {
                MessageBox.Show("Este permiso ya fue asignado.");
                return;
            }

            // Actualizar en XML
            var usuarios = UsuarioXmlService.Leer();
            var usuarioReal = usuarios.FirstOrDefault(u => u.ID == usuarioSeleccionado.ID);
            if (usuarioReal != null)
            {
                usuarioReal.Rol = usuarioSeleccionado.Rol;
                UsuarioXmlService.Guardar(usuarios);
            }

            MessageBox.Show("Permiso asignado correctamente.");
            CargarTreeViewRolesPermisosUsuario(usuarioSeleccionado);
        }


        private void btnQuitarPermisoUsuario_Click(object sender, EventArgs e)
        {
            if (tvUsuarios.SelectedNode?.Tag is not Usuario usuarioSeleccionado)
            {
                MessageBox.Show("Seleccioná un usuario.");
                return;
            }

            if (tvPermisos.SelectedNode?.Tag is not PermisoCompleto permisoSeleccionado)
            {
                MessageBox.Show("Seleccioná un permiso.");
                return;
            }

            if (usuarioSeleccionado.Rol is not PermisoCompuesto compuesto) return;

            var hijo = compuesto.ObtenerHijos().FirstOrDefault(p => p is PermisoCompleto pc && pc.ID == permisoSeleccionado.ID);
            if (hijo == null)
            {
                MessageBox.Show("El usuario no tiene ese permiso asignado.");
                return;
            }

            compuesto.Quitar(hijo);

            // Actualizar en XML
            var usuarios = UsuarioXmlService.Leer();
            var usuarioReal = usuarios.FirstOrDefault(u => u.ID == usuarioSeleccionado.ID);
            if (usuarioReal != null)
            {
                usuarioReal.Rol = compuesto;
                UsuarioXmlService.Guardar(usuarios);
            }

            MessageBox.Show("Permiso eliminado del usuario.");
            CargarTreeViewRolesPermisosUsuario(usuarioSeleccionado);
        }

        private void btnAsociarPermisoRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rolSeleccionado)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }

            if (tvPermisos.SelectedNode?.Tag is not PermisoCompleto permisoSeleccionado)
            {
                MessageBox.Show("Seleccioná un permiso.");
                return;
            }

            // Verificamos si ya estaba agregado
            if (!rolSeleccionado.ObtenerHijos().Any(p => p is PermisoCompleto pc && pc.Nombre == permisoSeleccionado.Nombre))
            {
                rolSeleccionado.Agregar(permisoSeleccionado);
                RolXmlService.Guardar(_roles);
                CargarTreeViewPermisosPorRol(rolSeleccionado);
                MessageBox.Show("Permiso asociado al rol.");
            }
            else
            {
                MessageBox.Show("Ese permiso ya está asignado a este rol.");
            }
        }

        private void CargarTreeViewPermisosPorRol(PermisoCompuesto rol)
        {
            tvPermisosPorRol.Nodes.Clear();

            TreeNode rolNode = new TreeNode(rol.Nombre);

            foreach (var permiso in rol.ObtenerHijos())
            {
                if (permiso is PermisoCompleto pc)
                {
                    TreeNode permisoNode = new TreeNode(pc.Nombre);

                    foreach (var menu in pc.MenuItems)
                    {
                        TreeNode menuNode = new TreeNode(menu.Menu);
                        foreach (var item in menu.Items)
                        {
                            menuNode.Nodes.Add(new TreeNode(item));
                        }
                        permisoNode.Nodes.Add(menuNode);
                    }

                    rolNode.Nodes.Add(permisoNode);
                }
            }

            tvPermisosPorRol.Nodes.Add(rolNode);
            tvPermisosPorRol.ExpandAll();
        }

        private void btnQuitarPermisoRol_Click(object sender, EventArgs e)
        {
            if (tvRoles.SelectedNode?.Tag is not PermisoCompuesto rolSeleccionado)
            {
                MessageBox.Show("Seleccioná un rol.");
                return;
            }

            if (tvPermisos.SelectedNode?.Tag is not PermisoCompleto permisoSeleccionado)
            {
                MessageBox.Show("Seleccioná un permiso.");
                return;
            }

            var hijos = rolSeleccionado.ObtenerHijos();
            var permisoARemover = hijos.FirstOrDefault(p => p is PermisoCompleto pc && pc.Nombre == permisoSeleccionado.Nombre);

            if (permisoARemover != null)
            {
                rolSeleccionado.Quitar(permisoARemover);
                RolXmlService.Guardar(_roles);
                CargarTreeViewPermisosPorRol(rolSeleccionado);
                MessageBox.Show("Permiso eliminado del rol.");
            }
            else
            {
                MessageBox.Show("El permiso no está asociado a este rol.");
            }
        }

    }


}
