using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class ComisionBLL
    {
        // repositorio que almacena las comisiones en un archivo XML
        private readonly XmlRepository<Comision> _repo = new("comisiones.xml");

        public void Registrar(Comision comision)
        {
            comision.ID = GeneradorID.ObtenerID<Comision>();
            comision.Fecha = DateTime.Now;
            _repo.Agregar(comision);
        }

        public List<Comision> ObtenerComisionesPorVendedorYFiltros(int idVendedor, string estado, DateTime desde, DateTime hasta)
        {
            return _repo.ObtenerTodos()
                        .Where(c =>
                            c.Venta?.Vendedor != null &&
                            c.Venta.Vendedor.ID == idVendedor &&
                            c.Estado == estado &&
                            c.Fecha.Date >= desde.Date &&
                            c.Fecha.Date <= hasta.Date)
                        .ToList();
        }

    }
}
