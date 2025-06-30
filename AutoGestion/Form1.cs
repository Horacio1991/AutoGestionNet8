using AutoGestion.Servicios; 
using AutoGestion.Servicios.Composite;
using AutoGestion.Vista;
using Vista.UserControls.Backup;
using Vista.UserControls.Dashboard;

namespace AutoGestion
{
    public partial class Form1 : Form
    {
        private readonly Usuario _usuario;
        public Form1(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var usuario = Sesion.UsuarioActual;
            if (usuario == null) return;

            // Si es admin, dejo todo visible y salgo
            if (usuario.Nombre.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return;

            // 1) Oculto todo inicialmente
            foreach (ToolStripMenuItem menu in menuPrincipal.Items)
                menu.Visible = false;

            // 2) Hago visible el menú Seguridad
            var seguridad = menuPrincipal.Items
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(m => m.Name == "seguridadToolStripMenuItem");
            if (seguridad != null)
            {
                seguridad.Visible = true;
                // Dentro de Seguridad, oculto todo excepto Cerrar Sesión
                foreach (ToolStripItem sub in seguridad.DropDownItems)
                    sub.Visible = (sub.Name == "mnuCerrarSesion");
            }

            // 3) Ahora muestro sólo los menús/submenús que tenga permiso el rol
            AplicarPermisos(usuario.Rol);
        }

        // Recorre el menu principal y oculta las opciones dependiendo el rol
        private void AplicarPermisos(IPermiso rol)
        {
            foreach (ToolStripMenuItem menu in menuPrincipal.Items)
            {
                // Saltar el menú Seguridad, ya lo manejamos arriba
                if (menu.Name == "seguridadToolStripMenuItem")
                    continue;

                // Visible si el rol tiene permiso para ese menú
                menu.Visible = TienePermiso(rol, menu.Text);

                // Luego los subitems
                foreach (var item in menu.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    item.Visible = TienePermiso(rol, item.Text);
                }
            }
        }

        // Devuelve true si el permiso contiene un PermisoCompleto,
        // donde el nombre o el menú coincidan con el texto proporcionado
        private bool TienePermiso(IPermiso permiso, string texto)
        {
            if (permiso == null || string.IsNullOrWhiteSpace(texto))
                return false;

            // Coincidencia directa
            if (string.Equals(permiso.Nombre, texto, StringComparison.OrdinalIgnoreCase))
                return true;

            // Si es compuesto, compruebo en todos sus hijos
            if (permiso is PermisoCompuesto pc)
            {
                foreach (var hijo in pc.Hijos) // Hijos = HijosCompuestos ∪ HijosSimples
                {
                    if (TienePermiso(hijo, texto))
                        return true;
                }
            }
            return false;
        }


        // Metodos para cargar UserControls en el panel de contenido
        private void mnuRegistrarCliente_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarCliente();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuSolicitarModelo_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new SolicitarModelo();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRealizarPago_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RealizarPago();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuAutorizarVenta_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new AutorizarVenta();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuEmitirFactura_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new EmitirFactura();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRealizarEntrega_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RealizarEntrega();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRegistrarOferta_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarOferta();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuEvaluarVehiculo_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new EvaluarEstado();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuTasarVehiculo_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new TasarVehiculo();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRegistrarCompra_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarDatos();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRegistrarComision_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarComision();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuConsultarComisiones_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new ConsultarComisiones();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRegistrarTurno_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarTurno();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuRegistrarAsistencia_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new RegistrarAsistencia();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuAsignarRoles_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new AsignarRoles();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void aBMUsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new ABMUsuarios();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void mnuCerrarSesion_Click_1(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("¿Seguro que querés cerrar sesión?", "Cerrar sesión", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                Sesion.UsuarioActual = null;
                FormLogin login = new FormLogin();
                login.Show();
                this.Close();
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new Dashboard();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);

        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new UC_Backup();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new UC_Restore();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);

        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenido.Controls.Clear();
            var control = new UC_Bitacora();
            control.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(control);
        }
    }
}



