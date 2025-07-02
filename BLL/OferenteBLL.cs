using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    // BLL para crear Oferentes en el sistema.
    public class OferenteBLL
    {
        private readonly XmlRepository<Oferente> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/oferentes.xml".
        public OferenteBLL()
        {
            _repo = new XmlRepository<Oferente>("oferentes.xml");
        }

        public Oferente BuscarPorDni(string dni)
        {
            try
            {
                // 1) Leer todos los oferentes
                var lista = _repo.ObtenerTodos();
                // 2) Buscar coincidencia exacta
                return lista.FirstOrDefault(o => o.Dni == dni);
            }
            catch (ApplicationException)
            {
                // 3) En caso de fallo, retornar null para no interrumpir la UI
                return null;
            }
        }

        public void GuardarOferente(Oferente oferente)
        {
            try
            {
                // 1) Asignar nuevo ID único usando el servicio central
                oferente.ID = GeneradorID.ObtenerID<Oferente>();
                // 2) Persistir en XML
                _repo.Agregar(oferente);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al guardar oferente: {ex.Message}", ex);
            }
        }
    }
}
