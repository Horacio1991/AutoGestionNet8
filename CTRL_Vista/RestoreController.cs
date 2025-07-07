using AutoGestion.Servicios.Backup;

namespace AutoGestion.CTRL_Vista
{
    public class RestoreController
    {
        private readonly BitacoraController _bitCtrl = new();

        // Devuelve la lista de backups disponibles (nombres de carpetas).
        public List<string> ObtenerBackups()
        {
            try
            {
                return BackupService.ObtenerBackups().ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al listar backups: {ex.Message}", ex);
            }
        }

        // Restaura el backup indicado y registra la acción en la bitácora.
        public void Restaurar(string nombreBackup, int usuarioId, string usuarioNombre)
        {
            try
            {
                BackupService.RestaurarBackup(nombreBackup);
                _bitCtrl.RegistrarEvento("restore", usuarioId, usuarioNombre);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al restaurar backup: {ex.Message}", ex);
            }
        }
    }
}
