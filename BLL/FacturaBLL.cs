using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class FacturaBLL
    {
        private readonly XmlRepository<Factura> _repo;


        public FacturaBLL()
        {
            _repo = new XmlRepository<Factura>("facturas.xml");
        }

        public Factura EmitirFactura(Factura factura)
        {
            try
            {
                factura.ID = GeneradorID.ObtenerID<Factura>();
                factura.Fecha = DateTime.Now;
                var lista = _repo.ObtenerTodos();
                lista.Add(factura);
                _repo.GuardarLista(lista);

                return factura;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException(
                    $"Error al emitir la factura: {ex.Message}", ex);
            }
        }
    }
}
