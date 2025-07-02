using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class ComprobanteBLL
    {
        private readonly XmlRepository<ComprobanteEntrega> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/comprobantes.xml".
        public ComprobanteBLL()
        {
            _repo = new XmlRepository<ComprobanteEntrega>("comprobantes.xml");
        }


        // Registra un nuevo comprobante de entrega.
        public void RegistrarComprobante(ComprobanteEntrega comp)
        {
            try
            {
                // 1) Asignar ID único
                comp.ID = GeneradorID.ObtenerID<ComprobanteEntrega>();
                // 2) Persistir el comprobante en XML
                _repo.Agregar(comp);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar comprobante: {ex.Message}", ex);
            }
        }

        // Obtiene todos los comprobantes de entrega registrados.
        public List<ComprobanteEntrega> ObtenerTodos()
        {
            try
            {
                // 1) Leer todos los comprobantes
                return _repo.ObtenerTodos();
            }
            catch (ApplicationException)
            {
                // 2) Devolver lista vacía para no interrumpir la UI
                return new List<ComprobanteEntrega>();
            }
        }
    }
}
