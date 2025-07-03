using AutoGestion.DAO.Repositorios;
using Entidades;


namespace AutoGestion.Servicios
{
    public static class BitacoraService
    {
        // Repositorio para manejar la persistencia de Bitacora en DatosXML/bitacora.xml
        private static readonly XmlRepository<Bitacora> _repo = new("bitacora.xml");

        // Registra un nuevo evento en la bitácora.
        // detalle => Detalle de la acción (por ejemplo "backup" o "restore")
        // usuarioID => ID del usuario que realiza la acción
        // usuarioNombre => Nombre del usuario que realiza la acción
        public static void Registrar(string detalle, int usuarioID, string usuarioNombre)
        {
            if (string.IsNullOrWhiteSpace(detalle))
                throw new ArgumentException("Detalle de bitácora requerido.", nameof(detalle));

            try
            {
                var entrada = new Bitacora
                {
                    FechaRegistro = DateTime.Now,
                    Detalle = detalle.ToLowerInvariant(),
                    UsuarioID = usuarioID,
                    UsuarioNombre = usuarioNombre
                };
                _repo.Agregar(entrada);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar en bitácora: {ex.Message}", ex);
            }
        }

        // Obtiene todas las entradas de la bitácora, ordenadas
        // por fecha de registro descendente (más recientes primero).
        public static List<Bitacora> ObtenerTodo()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .OrderByDescending(b => b.FechaRegistro)
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al leer bitácora: {ex.Message}", ex);
            }
        }
    }
}
