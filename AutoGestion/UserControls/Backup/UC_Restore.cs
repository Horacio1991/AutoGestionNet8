using AutoGestion.Servicios;
using BLL;


namespace Vista.UserControls.Backup
{
    public partial class UC_Restore : UserControl
    {
        // Carpeta donde se guardan los backups
        private readonly string rutaBackups = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        // Carpeta donde se guardan los XML de datos de la aplicación       
        private readonly string rutaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
        
        private readonly BitacoraBLL _bitacoraBLL = new();

        public UC_Restore()
        {
            InitializeComponent();
            CargarBackups();
        }

        private void CargarBackups()
        {
            // Verificar si la carpeta de backups existe, si no, crearla
            if (!Directory.Exists(rutaBackups))
                Directory.CreateDirectory(rutaBackups);

            // Toma el nombre de las carpetas dentro de la ruta de backups, ordenadas por fecha (nombre de carpeta)
            var carpetas = Directory.GetDirectories(rutaBackups)
                                    .Select(f => new DirectoryInfo(f).Name)
                                    .OrderByDescending(f => f)
                                    .ToArray();

            lstBackups.Items.Clear();
            lstBackups.Items.AddRange(carpetas);
        }

        private void btnRestaurarSeleccionado_Click(object sender, EventArgs e)
        {
            // Verificar si se ha seleccionado un backup
            if (lstBackups.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná un backup para restaurar.");
                return;
            }

            // Contruye rutas de origen (backup) y destino (datos de la aplicación)
            string backupSeleccionado = lstBackups.SelectedItem.ToString();
            string rutaBackupSeleccionado = Path.Combine(rutaBackups, backupSeleccionado);

            try
            {
                //Obtener todos los archivos del backup seleccionado
                var archivosBackup = Directory.GetFiles(rutaBackupSeleccionado);

                // Recorrer los archivos del backup y copiarlos al directorio de datos de la aplicación
                foreach (var archivo in archivosBackup)
                {
                    string nombreArchivo = Path.GetFileName(archivo);

                    if (nombreArchivo.Equals("bitacora.xml", StringComparison.OrdinalIgnoreCase))
                        continue; // Nunca restaurar la bitacora

                    string destino = Path.Combine(rutaDatos, nombreArchivo);
                    File.Copy(archivo, destino, overwrite: true);
                }

                // Registrar Restore en Bitacora
                var usuario = Sesion.UsuarioActual;
                _bitacoraBLL.Registrar("restore", usuario?.ID ?? 0, usuario?.Nombre ?? "Desconocido");

                MessageBox.Show("Restore realizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al restaurar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
    }
}
