using AutoGestion.DAO.Repositorios;
using Entidades;


namespace AutoGestion.Servicios
{
    public static class BitacoraService
    {
        // Repositorio para manejar la bitácora de eventos (Ruta DatosXML/bitacora.xml)
        private static readonly XmlRepository<Bitacora> _repo = new("bitacora.xml");

        public static void Registrar(string detalle, int usuarioID, string usuarioNombre)
        {
            // Crea una nueva entrada de bitácora con la fecha actual, el detalle, el ID y nombre del usuario
            var entrada = new Bitacora
            {
                FechaRegistro = DateTime.Now,
                Detalle = detalle,
                UsuarioID = usuarioID,
                UsuarioNombre = usuarioNombre
            };
            // Agrega la entrada al repositorio
            _repo.Agregar(entrada);
        }

        public static List<Bitacora> ObtenerTodo()
        {
            // Obtiene todas las entradas de la bitácora, ordenadas por fecha de registro de forma descendente
            return _repo.ObtenerTodos().OrderByDescending(b => b.FechaRegistro).ToList();
        }
    }
}
