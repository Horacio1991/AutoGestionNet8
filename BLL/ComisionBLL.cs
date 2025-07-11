using AutoGestion.DAO.Repositorios;
using AutoGestion.Entidades;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.BLL
{
    public class ComisionBLL
    {
        private readonly XmlRepository<Comision> _repo;
        private readonly VentaBLL _ventaBll;

        // 1) Inicializa el repositorio apuntando a "DatosXML/comisiones.xml".
        public ComisionBLL()
        {
            _repo = new XmlRepository<Comision>("comisiones.xml");
            _ventaBll = new VentaBLL();
        }

        public bool RegistrarComision(Comision comision)
        {
            try
            {
                // 1) Asignar ID único
                comision.ID = GeneradorID.ObtenerID<Comision>();
                // 2) Fijar fecha actual
                comision.Fecha = DateTime.Now;
                // 3) Persistir en XML
                _repo.Agregar(comision);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar comisión: {ex.Message}", ex);
            }
        }

        // Obtiene las ventas ya entregadas que aún no tienen comisión.
        // Usado para identificar ventas pendientes de comisionar.
        public List<Venta> ObtenerVentasSinComision()
        {
            try
            {
                // 1) Traer ventas entregadas
                var entregadas = _ventaBll.ObtenerVentasEntregadas();
                // 2) IDs de ventas con comisión. Usamos HashSet para búsqueda rápida.
                var idsConCom = _repo.ObtenerTodos()
                                    .Select(c => c.Venta.ID)
                                    .ToHashSet();
                // 3) Filtrar las que no están en comisiones
                return entregadas
                       .Where(v => !idsConCom.Contains(v.ID))
                       .ToList();
            }
            catch (ApplicationException)
            {
                // En caso de error, devolver lista vacía
                return new List<Venta>();
            }
        }

        // Obtiene comisiones de un vendedor según estado y rango de fechas.
        public List<Comision> ObtenerComisionesPorVendedorYFiltros(
            int vendedorId,
            string estado,
            DateTime desde,
            DateTime hasta)
        {
            try
            {
                // 1) Leer todas las comisiones
                var todas = _repo.ObtenerTodos();
                // 2) Filtrar por vendedor, estado y rango de fechas
                var filtradas = todas
                    .Where(c =>
                        c.Venta?.Vendedor?.ID == vendedorId && //Evalua que venta y vendedor no sean nulos
                        string.Equals(c.Estado, estado, StringComparison.OrdinalIgnoreCase) && //Compara c.Estado con el estado proporcionado por parametro)
                        c.Fecha.Date >= desde.Date &&
                        c.Fecha.Date <= hasta.Date
                    )
                    .OrderByDescending(c => c.Fecha)
                    .ToList();
                return filtradas;
            }
            catch (ApplicationException)
            {
                // En caso de error, devolver lista vacía
                return new List<Comision>();
            }
        }
    }
}
