using System;
using System.IO;

namespace AutoGestion.Servicios.Backup
{
    public static class BackupService
    {
        // Donde voy a guardar los backups
        private static readonly string CarpetaPrincipal =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        // Carpeta donde estan los datos XML 
        private static readonly string DatosDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");

        public static string RealizarBackup()
        {
            // 1) Asegurar carpeta de backups
            if (!Directory.Exists(CarpetaPrincipal))
                Directory.CreateDirectory(CarpetaPrincipal);

            // 2) Crear subcarpeta con timestamp
            string nombreCarpeta = $"BD_Backup_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}";
            string destino = Path.Combine(CarpetaPrincipal, nombreCarpeta);
            Directory.CreateDirectory(destino);

            // 3) Copiar todos los XML de DatosXML excepto bitacora.xml
            if (!Directory.Exists(DatosDir))
                throw new DirectoryNotFoundException($"No existe la carpeta de datos: {DatosDir}");

            var archivos = Directory.GetFiles(DatosDir, "*.xml");
            foreach (var archivo in archivos)
            {
                var nombre = Path.GetFileName(archivo);
                if (string.Equals(nombre, "bitacora.xml", StringComparison.OrdinalIgnoreCase))
                    continue;
                File.Copy(archivo, Path.Combine(destino, nombre), overwrite: true);
            }

            return nombreCarpeta;
        }

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
                throw new DirectoryNotFoundException($"Backup no encontrado: {nombreBackup}");

            // 1) Asegurar DatosDir
            if (!Directory.Exists(DatosDir))
                Directory.CreateDirectory(DatosDir);

            // 2) Copiar, excepto bitacora.xml
            foreach (var archivo in Directory.GetFiles(carpetaOrigen))
            {
                var nombre = Path.GetFileName(archivo);
                if (string.Equals(nombre, "bitacora.xml", StringComparison.OrdinalIgnoreCase))
                    continue;
                File.Copy(archivo, Path.Combine(DatosDir, nombre), overwrite: true);
            }
        }
    }
}
