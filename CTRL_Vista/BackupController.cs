using AutoGestion.BLL;
using AutoGestion.Servicios.Backup;

namespace AutoGestion.CTRL_Vista
{
    // Controller para manejar la lógica de Backup / Restore
    public class BackupController
    {
        private readonly BitacoraBLL _bitacoraBll = new();

        // Realiza un backup de los archivos XML (excepto la bitácora),
        // registra la operación en la bitácora y devuelve el nombre de la carpeta creada.
        public string RealizarBackup(int usuarioId, string usuarioNombre)
        {
            // 1) Llamar al servicio que copia los XML
            string carpeta = BackupService.RealizarBackup();

            // 2) Registrar en bitácora
            _bitacoraBll.Registrar("backup", usuarioId, usuarioNombre);

            return carpeta;
        }

        // Devuelve el listado de backups disponibles (nombres de carpetas, ordenados desc.).
        public List<string> ObtenerHistorial()
        {
            return new List<string>(BackupService.ObtenerBackups());
        }

        // Restaura el backup indicado (por nombre de carpeta),
        // y registra la operación en la bitácora.
        public void RestaurarBackup(string carpeta, int usuarioId, string usuarioNombre)
        {
            BackupService.RestaurarBackup(carpeta);
            _bitacoraBll.Registrar("restore", usuarioId, usuarioNombre);
        }
    }
}
