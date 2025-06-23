using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;

namespace AutoGestion.BLL
{
    public class ComprobanteBLL
    {
        private readonly XmlRepository<ComprobanteEntrega> _repo = new("comprobantes.xml");

        public void RegistrarComprobante(ComprobanteEntrega comp)
        {
            comp.ID = ObtenerNuevoID();
            _repo.Agregar(comp);
        }

        private int ObtenerNuevoID()
        {
            var lista = _repo.ObtenerTodos();
            return lista.Any() ? lista.Max(c => c.ID) + 1 : 1;
        }

        public List<ComprobanteEntrega> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
    }
}
