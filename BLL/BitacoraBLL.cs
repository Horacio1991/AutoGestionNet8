using Entidades;
using AutoGestion.DAO.Repositorios;

namespace BLL
{
    public class BitacoraBLL
    {
        // Repositorio para manejar la persistencia de la bitácora en "DatosXML/bitacora.xml"
        private readonly XmlRepository<Bitacora> _repo;

        // Constructor que inicializa el repositorio con el archivo XML de la bitácora
        public BitacoraBLL()
        {
            _repo = new XmlRepository<Bitacora>("bitacora.xml");
        }

        // Registrar eventos de backup o restore en la bitácora
        public void Registrar(string tipo, int usuarioID, string usuarioNombre)
        {
            // Crea un nuevo registro de bitácora con la fecha actual, tipo de acción, ID y nombre del usuario
            var registro = new Bitacora
            {
                FechaRegistro = DateTime.Now,
                Detalle = tipo.ToLower(), // "backup" o "restore"
                UsuarioID = usuarioID,
                UsuarioNombre = usuarioNombre
            };
            // Agrega el registro al repositorio, que lo serializa y guarda en el archivo XML
            _repo.Agregar(registro);
        }

        // Devuelve todos los registros de la bitácora sin filtrar
        public List<Bitacora> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
    }
}
