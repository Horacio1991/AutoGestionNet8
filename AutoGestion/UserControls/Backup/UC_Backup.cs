using AutoGestion.BLL;
using AutoGestion.Servicios;


namespace Vista.UserControls.Backup
{
    public partial class UC_Backup : UserControl
    {
        // Carpeta donde se guardan los datos XML actuales
        private readonly string rutaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
        // Carpeta principal donde se guardan los backups
        private readonly string rutaBackups = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        
        private readonly BitacoraBLL _bitacoraBLL = new();

        public UC_Backup()
        {
            InitializeComponent();
            CargarHistorial();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                // me aseguro que la carpeta de backups exista
                if (!Directory.Exists(rutaBackups))
                    Directory.CreateDirectory(rutaBackups);

                // Nombre de carpeta con timestamp
                string carpetaBackup = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string destinoBackup = Path.Combine(rutaBackups, carpetaBackup);
                Directory.CreateDirectory(destinoBackup);

                // Copiar todos los XML excepto bitacora.xml
                var archivos = Directory.GetFiles(rutaDatos, "*.xml")
                                        .Where(a => !a.EndsWith("bitacora.xml", StringComparison.OrdinalIgnoreCase));

                //Copiar los archivos a la nueva carpeta de backup
                foreach (var archivo in archivos)
                {
                    string nombre = Path.GetFileName(archivo);
                    File.Copy(archivo, Path.Combine(destinoBackup, nombre), overwrite: true);
                }

                // Registrar backup en bitácora
                var usuario = Sesion.UsuarioActual; //Recuperar el usuario actual de la sesión
               // Si usuario no es null , registrar el backup
                _bitacoraBLL.Registrar("backup", usuario?.ID ?? 0, usuario?.Nombre ?? "Desconocido");

                MessageBox.Show("Backup realizado con éxito.");
                CargarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al hacer backup: " + ex.Message);
            }
        }

        private void CargarHistorial()
        {
            dgvBackup.Rows.Clear();
            dgvBackup.Columns.Clear();

            dgvBackup.Columns.Add("Fecha", "Fecha");
            dgvBackup.Columns.Add("Usuario", "Usuario");

            var bitacora = new BitacoraBLL().ObtenerTodos()
                                            .Where(b => b.Detalle == "backup")
                                            .OrderByDescending(b => b.FechaRegistro);

            foreach (var b in bitacora)
            {
                dgvBackup.Rows.Add(b.FechaRegistro.ToString("g"), b.UsuarioNombre);
            }

            dgvBackup.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBackup.ReadOnly = true;
        }
    }
}
