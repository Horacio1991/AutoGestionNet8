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
    }
}
