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
            Load += Form1_Load;
        }

        /// Al cargar la ventana principal:
        /// 1) Verifica sesión válida.
        /// 2) Oculta/muestra menús según rol.
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (_usuario == null)
                    throw new ApplicationException("Sesión inválida. Por favor, ingrese nuevamente.");

                // Si es admin, mostramos todo
                if (_usuario.Nombre.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    return;

                // Ocultar todos los menús inicialmente
                foreach (ToolStripMenuItem menu in menuPrincipal.Items)
                    menu.Visible = false;

                // Mostrar solo “Seguridad” (cerrar sesión) para todos
                var seguridad = menuPrincipal.Items
                    .OfType<ToolStripMenuItem>()
                    .FirstOrDefault(m => m.Name == "seguridadToolStripMenuItem");
                if (seguridad != null)
                {
                    seguridad.Visible = true;
                    foreach (ToolStripItem sub in seguridad.DropDownItems)
                        sub.Visible = (sub.Name == "mnuCerrarSesion");
                }

                // Aplicar visibilidad según permisos del rol
                AplicarPermisos(_usuario.Rol);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar la aplicación: {ex.Message}",
                    "Error crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        /// Recorre todos los menús y submenús:
        /// - Muestra solo aquellos cuyo texto coincide con un permiso en el rol.
        private void AplicarPermisos(IPermiso rol)
        {
            foreach (ToolStripMenuItem menu in menuPrincipal.Items)
            {
                if (menu.Name == "seguridadToolStripMenuItem")
                    continue;

                menu.Visible = TienePermiso(rol, menu.Text);

                foreach (ToolStripMenuItem sub in menu.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    sub.Visible = TienePermiso(rol, sub.Text);
                }
            }
        }

        /// Comprueba recursivamente si el permiso (hoja o compuesto)
        /// coincide con el texto de menú o submenú.
        private bool TienePermiso(IPermiso permiso, string texto)
        {
            if (permiso == null || string.IsNullOrWhiteSpace(texto))
                return false;

            if (permiso.Nombre.Equals(texto, StringComparison.OrdinalIgnoreCase))
                return true;

            if (permiso is PermisoCompuesto pc)
            {
                return pc.Hijos.Any(h => TienePermiso(h, texto));
            }

            return false;
        }

        // Métodos para cargar cada UserControl en el panel de contenido
        private void mnuRegistrarCliente_Click_1(object sender, EventArgs e) => CargarControl(new RegistrarCliente());
        private void mnuSolicitarModelo_Click_1(object sender, EventArgs e) => CargarControl(new SolicitarModelo());
        private void mnuRealizarPago_Click_1(object sender, EventArgs e) => CargarControl(new RealizarPago());
        private void mnuAutorizarVenta_Click_1(object sender, EventArgs e) => CargarControl(new AutorizarVenta());
        private void mnuEmitirFactura_Click_1(object sender, EventArgs e) => CargarControl(new EmitirFactura());
        private void mnuRealizarEntrega_Click_1(object sender, EventArgs e) => CargarControl(new RealizarEntrega());
        private void mnuRegistrarOferta_Click_1(object sender, EventArgs e) => CargarControl(new RegistrarOferta());
        private void mnuEvaluarVehiculo_Click_1(object sender, EventArgs e) => CargarControl(new EvaluarEstado());
        private void mnuTasarVehiculo_Click_1(object sender, EventArgs e) => CargarControl(new TasarVehiculo());
        private void mnuRegistrarCompra_Click_1(object sender, EventArgs e) => CargarControl(new RegistrarDatos());
        private void mnuRegistrarComision_Click(object sender, EventArgs e) => CargarControl(new RegistrarComision());
        private void mnuConsultarComisiones_Click_1(object sender, EventArgs e) => CargarControl(new ConsultarComisiones());
        private void mnuRegistrarTurno_Click_1(object sender, EventArgs e) => CargarControl(new RegistrarTurno());
        private void mnuRegistrarAsistencia_Click_1(object sender, EventArgs e) => CargarControl(new RegistrarAsistencia());
        private void mnuAsignarRoles_Click(object sender, EventArgs e) => CargarControl(new AsignarRoles());
        private void aBMUsToolStripMenuItem_Click_1(object sender, EventArgs e) => CargarControl(new ABMUsuarios());
        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e) => CargarControl(new Dashboard());
        private void backupToolStripMenuItem_Click(object sender, EventArgs e) => CargarControl(new UC_Backup());
        private void restoreToolStripMenuItem_Click(object sender, EventArgs e) => CargarControl(new UC_Restore());
        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e) => CargarControl(new UC_Bitacora());

        // Lógica común para cargar un UserControl en el panel principal.
        private void CargarControl(UserControl control)
        {
            try
            {
                panelContenido.Controls.Clear();
                control.Dock = DockStyle.Fill;
                panelContenido.Controls.Add(control);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar la pantalla: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Evento de Cerrar Sesión: limpia la sesión y vuelve al login.
        private void mnuCerrarSesion_Click_1(object sender, EventArgs e)
        {
            try
            {
                var confirm = MessageBox.Show(
                    "¿Seguro que querés cerrar sesión?",
                    "Cerrar sesión",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    Sesion.UsuarioActual = null;
                    new FormLogin().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cerrar sesión: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
