using AutoGestion.Servicios;
using AutoGestion.Servicios.Composite;
using AutoGestion.Vista;
using Vista.UserControls.Backup;
using Vista.UserControls.Dashboard;

namespace AutoGestion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var usuario = Sesion.UsuarioActual;

            if (usuario == null) return;

            if (usuario.Nombre.ToLower() == "admin")
                return;

            AplicarPermisos(usuario.Rol);
        }

        private void AplicarPermisos(IPermiso rol)
        {
            if (rol == null)
            {
                foreach (ToolStripMenuItem menu in menuPrincipal.Items)
                {
                    if (menu.Name != "mnuCerrarSesion")
                        menu.Visible = false;
                }
                return;
            }

            foreach (ToolStripMenuItem menu in menuPrincipal.Items)
            {
                if (menu.Name == "mnuCerrarSesion") continue;

                bool visible = TienePermiso(rol, menu.Text);
                menu.Visible = visible;

                foreach (ToolStripItem subItem in menu.DropDownItems)
                {
                    subItem.Visible = TienePermiso(rol, subItem.Text);
                }
            }
        }

        private bool TienePermiso(IPermiso rol, string texto)
        {
            if (rol == null || string.IsNullOrWhiteSpace(texto))
                return false;

            foreach (var hijo in rol.ObtenerHijos())
            {
                if (hijo is PermisoCompleto pc)
                {
                    // Coincidencia directa por nombre
                    if (string.Equals(pc.Nombre, texto, StringComparison.OrdinalIgnoreCase))
                        return true;

                    // Coincidencia por menú
                    foreach (var menu in pc.MenuItems)
                    {
                        if (string.Equals(menu.Menu, texto, StringComparison.OrdinalIgnoreCase))
                            return true;

                        if (menu.Items.Any(item => string.Equals(item, texto, StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }
                }
            }

            return false;
        }

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



