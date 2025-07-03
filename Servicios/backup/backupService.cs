namespace AutoGestion.Servicios.Backup
{
    // Servicio que realiza backups y restores
    // de los archivos de datos XML (sin incluir la bitácora).
    public static class BackupService
    {
        // Carpeta principal donde se guardan los backups, junto al ejecutable.
        private static readonly string CarpetaPrincipal =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        /// Realiza un backup de todos los archivos XML
        /// (salvo bitacora.xml) copiándolos a una subcarpeta con timestamp.
        public static string RealizarBackup()
        {
            try
            {
                // 1) Asegurarnos de que exista la carpeta principal
                Directory.CreateDirectory(CarpetaPrincipal);

                // 2) Generar nombre con fecha y hora
                string nombreCarpeta = $"BD_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
                string destino = Path.Combine(CarpetaPrincipal, nombreCarpeta);

                // 3) Crear carpeta del backup
                Directory.CreateDirectory(destino);

                // 4) Copiar cada XML, excepto bitacora.xml
                var archivos = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml")
                                        .Where(f => !f.EndsWith("bitacora.xml", StringComparison.OrdinalIgnoreCase));

                foreach (var archivo in archivos)
                {
                    string nombre = Path.GetFileName(archivo);
                    string rutaDestino = Path.Combine(destino, nombre);
                    File.Copy(archivo, rutaDestino, overwrite: true);
                }

                return nombreCarpeta;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al realizar backup: {ex.Message}", ex);
            }
        }

        // Devuelve la lista de nombres de carpetas de backup existentes,
        // ordenadas de más reciente a más antiguo.
        public static string[] ObtenerBackups()
        {
            try
            {
                if (!Directory.Exists(CarpetaPrincipal))
                    return Array.Empty<string>();

                return Directory.GetDirectories(CarpetaPrincipal)
                                .Select(Path.GetFileName)
                                .OrderByDescending(n => n)
                                .ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al listar backups: {ex.Message}", ex);
            }
        }

        // Restaura un backup específico copiando sus archivos
        // (salvo bitacora.xml) de vuelta a la carpeta de datos.
        public static void RestaurarBackup(string nombreBackup)
        {
            try
            {
                string carpetaOrigen = Path.Combine(CarpetaPrincipal, nombreBackup);
                if (!Directory.Exists(carpetaOrigen))
                    throw new DirectoryNotFoundException("La carpeta de backup no existe.");

                var archivosBackup = Directory.GetFiles(carpetaOrigen);
                foreach (var archivo in archivosBackup)
                {
                    string nombre = Path.GetFileName(archivo);

                    // Nunca sobreescribir la bitácora
                    if (nombre.Equals("bitacora.xml", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string destino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nombre);
                    File.Copy(archivo, destino, overwrite: true);
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw; // Propagar sin envolver para distinguir caso de no existe carpeta
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al restaurar backup: {ex.Message}", ex);
            }
        }
    }
}
