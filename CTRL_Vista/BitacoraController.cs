using AutoGestion.Servicios;
using Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class BitacoraController
    {
        // Registra un evento en bitácora.
        public void RegistrarEvento(string detalle, int usuarioID, string usuarioNombre)
        {
            try
            {
                BitacoraService.Registrar(detalle, usuarioID, usuarioNombre);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"No se pudo registrar el evento en bitácora: {ex.Message}", ex);
            }
        }

        // Recupera todos los registros de bitácora.
        public List<Bitacora> ObtenerRegistros()
        {
            try
            {
                return BitacoraService.ObtenerTodo();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"No se pudieron cargar los registros de bitácora: {ex.Message}", ex);
            }
        }
    }
}
