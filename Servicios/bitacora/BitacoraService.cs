using AutoGestion.DAO.Repositorios;
using Entidades;

namespace AutoGestion.Servicios
{
    public static class BitacoraService
    {
        private static readonly XmlRepository<Bitacora> _repo = new("bitacora.xml");

        public static void Registrar(string detalle, int usuarioID, string usuarioNombre)
        {
            try
            {
                var entrada = new Bitacora
                {
                    FechaRegistro = DateTime.Now,
                    Detalle = detalle,
                    UsuarioID = usuarioID,
                    UsuarioNombre = usuarioNombre
                };
                _repo.Agregar(entrada);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar en la bitácora: {ex.Message}", ex);
            }
        }

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
                throw new ApplicationException($"Error al leer la bitácora: {ex.Message}", ex);
            }
        }
    }
}
