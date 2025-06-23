using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.BLL
{
    public class FacturaBLL
    {
        // Repositorio que persiste List<Factura> en DatosXML/facturas.xml
        private readonly XmlRepository<Factura> _repo = new("facturas.xml");

        public Factura EmitirFactura(Factura factura)
        {
            factura.ID = GeneradorID.ObtenerID<Factura>();
            factura.Fecha = DateTime.Now;

            var lista = _repo.ObtenerTodos();
            lista.Add(factura);
            _repo.GuardarLista(lista);

            return factura;
        }

       
    }
}
