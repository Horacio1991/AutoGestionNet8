using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;


namespace AutoGestion.BLL
{
    public class PagoBLL
    {
        // Repositorio genérico que persiste List<Pago> en "DatosXML/pagos.xml"
        private readonly XmlRepository<Pago> _repo = new("pagos.xml");

        
        public bool RegistrarPago(Pago pago)
        {
            var lista = _repo.ObtenerTodos();
            lista.Add(pago);
            _repo.GuardarLista(lista);
            return true;
        }

    }
}
