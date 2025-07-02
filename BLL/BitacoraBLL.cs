using AutoGestion.DAO.Repositorios;
using Entidades;


namespace AutoGestion.BLL
{
    public class BitacoraBLL
    {
        private readonly XmlRepository<Bitacora> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/bitacora.xml".
        public BitacoraBLL()
        {
            _repo = new XmlRepository<Bitacora>("bitacora.xml");
        }

        // Registra un evento en la bitácora.
        // tipo = Tipo de evento ("backup" o "restore").
        // usuarioId = El usuario que realiza la acción.
        // usuarioNombre = Nombre del usuario que realiza la acción.
        public void Registrar(string tipo, int usuarioID, string usuarioNombre)
        {
            try
            {
                // 1) Crear registro
                var registro = new Bitacora
                {
                    FechaRegistro = DateTime.Now,
                    Detalle = tipo.ToLower(),
                    UsuarioID = usuarioID,
                    UsuarioNombre = usuarioNombre
                };

                // 2) Persistir en XML
                _repo.Agregar(registro);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is System.IO.IOException)
            {
                // 3) En caso de fallo, convertimos a ApplicationException
                throw new ApplicationException(
                    $"Error al registrar en la bitácora: {ex.Message}", ex);
            }
        }

        // Obtiene todos los registros de la bitácora.
        public List<Bitacora> ObtenerTodos()
        {
            try
            {
                // 1) Leer todos los registros
                var lista = _repo.ObtenerTodos();

                // 2) Ordenar por fecha (más reciente primero)
                lista.Sort((a, b) => b.FechaRegistro.CompareTo(a.FechaRegistro));

                return lista;
            }
            catch (ApplicationException)
            {
                // 3) Retornar lista vacía para que la UI no caiga
                return new List<Bitacora>();
            }
        }
    }
}
