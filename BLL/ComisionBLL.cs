using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class ComisionBLL
    {
        private readonly XmlRepository<Comision> _repo = new XmlRepository<Comision>("comisiones.xml");
        private readonly VentaBLL _ventaBll = new VentaBLL();

        /// <summary>
        /// Registra la comisión en XML.
        /// </summary>
        public bool RegistrarComision(Comision comision)
        {
            comision.ID = GeneradorID.ObtenerID<Comision>();
            comision.Fecha = DateTime.Now;
            _repo.Agregar(comision);
            return true;
        }

        /// <summary>
        /// Devuelve las ventas entregadas que aún no tienen comisión.
        /// </summary>
        public List<Venta> ObtenerVentasSinComision()
        {
            var entregadas = _ventaBll.ObtenerVentasEntregadas();
            var conCom = _repo.ObtenerTodos()
                              .Select(c => c.Venta.ID)
                              .ToHashSet();
            return entregadas
                   .Where(v => !conCom.Contains(v.ID))
                   .ToList();
        }

        /// <summary>
        /// Trae las comisiones de un vendedor dentro de un rango de fechas y estado.
        /// </summary>
        public List<Comision> ObtenerComisionesPorVendedorYFiltros(
            int vendedorId,
            string estado,
            DateTime desde,
            DateTime hasta)
        {
            // Leer todas
            var todas = _repo.ObtenerTodos();

            // Filtrar
            return todas
                .Where(c =>
                    c.Venta?.Vendedor != null &&
                    c.Venta.Vendedor.ID == vendedorId &&
                    c.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase) &&
                    c.Fecha.Date >= desde.Date &&
                    c.Fecha.Date <= hasta.Date
                )
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }
    }


}

