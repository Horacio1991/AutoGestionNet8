namespace Servicios.backup
{
    public static class BackupService
    {
        //Carpeta donde se guardan los backups
        private static readonly string CarpetaPrincipal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        public static string RealizarBackup()
        {
            // Verificar si la carpeta principal existe, sino la crea
            if (!Directory.Exists(CarpetaPrincipal))
                Directory.CreateDirectory(CarpetaPrincipal);

            // Nombre de la carpeta de backup con timestamp
            string nombreCarpeta = $"BD_Backup_{DateTime.Now:dd-MM-yyyy HH-mm-ss}";
            string destino = Path.Combine(CarpetaPrincipal, nombreCarpeta);
            //Crea la subcarpeta destino
            Directory.CreateDirectory(destino);

            // Copiar todos los archivos XML excepto bitacora.xml
            var archivos = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");

            //Recorremos los archivos y copiamos (excepto bitacora.xml)
            foreach (var archivo in archivos)
            {
                string nombre = Path.GetFileName(archivo);
                if (nombre.ToLower() == "bitacora.xml") continue; //no copiamos la bitácora

                File.Copy(archivo, Path.Combine(destino, nombre), overwrite: true);
            }

            return nombreCarpeta;
        }

        // Obtiene una lista de los backups disponibles
        public static string[] ObtenerBackups()
        {
            if (!Directory.Exists(CarpetaPrincipal))
                return Array.Empty<string>();

            return Directory.GetDirectories(CarpetaPrincipal)
                            .Select(Path.GetFileName)
                            .OrderByDescending(n => n)
                            .ToArray();
        }

        public static void RestaurarBackup(string nombreBackup)
        {
            string carpetaOrigen = Path.Combine(CarpetaPrincipal, nombreBackup);
            if (!Directory.Exists(carpetaOrigen))
                throw new DirectoryNotFoundException("La carpeta de backup no existe.");

            var archivosBackup = Directory.GetFiles(carpetaOrigen);

            foreach (var archivo in archivosBackup)
            {
                string nombre = Path.GetFileName(archivo);
                string destino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nombre);

                // ⚠️ Evitar sobrescribir la bitácora
                if (nombre.ToLower() == "bitacora.xml") continue;

                File.Copy(archivo, destino, overwrite: true);
            }
        }

    }
}
