using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class VentaBLL
    {
        private readonly XmlRepository<Venta> _repo;
        private readonly VehiculoBLL _vehiculoBLL;

        public VentaBLL()
        {
            _repo = new XmlRepository<Venta>("ventas.xml");
            _vehiculoBLL = new VehiculoBLL(); 
        }

        // Obtiene todas las ventas registradas.
        public List<Venta> ObtenerTodas()
        {
            try
            {
                return _repo.ObtenerTodos();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Obtiene todas las ventas pendientes de facturación.
        public List<Venta> ObtenerVentasPendientes()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Estado != "Facturada")
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Obtiene todas las ventas cuyo estado es "Pendiente".
        public List<Venta> ObtenerVentasConEstadoPendiente()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Estado == "Pendiente")
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Busca la venta con el ID dado.
        public Venta ObtenerDetalleVenta(int id)
        {
            try
            {
                return _repo.ObtenerTodos()
                            .FirstOrDefault(v => v.ID == id);
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        // Autoriza una venta si no hay otra autorizada para el mismo vehículo.
        public bool AutorizarVenta(int ventaId)
        {
            try
            {
                // 1) Leer lista
                var lista = _repo.ObtenerTodos().ToList();
                var venta = lista.FirstOrDefault(v => v.ID == ventaId);
                if (venta == null) return false;

                // 2) Verificar si ya existe autorizada para mismo dominio
                bool yaVendida = lista.Any(v =>
                    v.Estado == "Autorizada" &&
                    v.Vehiculo?.Dominio == venta.Vehiculo?.Dominio);

                // 3) Marcar estado
                if (yaVendida)
                {
                    venta.Estado = "Rechazada";
                    venta.MotivoRechazo = "Vehículo ya vendido en otra operación.";
                    _repo.GuardarLista(lista);
                    return false;
                }

                venta.Estado = "Autorizada";
                _repo.GuardarLista(lista);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al autorizar venta: {ex.Message}", ex);
            }
        }

        public bool RechazarVenta(int ventaId, string motivo)
        {
            try
            {
                var lista = _repo.ObtenerTodos().ToList();
                var venta = lista.FirstOrDefault(v => v.ID == ventaId);
                if (venta == null) return false;

                venta.Estado = "Rechazada";
                venta.MotivoRechazo = motivo;

                // Actualizar estado del vehículo a "Disponible"
                _vehiculoBLL.ActualizarEstadoVehiculo(venta.Vehiculo, VehiculoEstados.Disponible);

                // Liberar vehículo
                _repo.GuardarLista(lista);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al rechazar venta: {ex.Message}", ex);
            }
        }

        public void RegistrarVenta(Venta venta)
        {
            try
            {
                venta.ID = GeneradorID.ObtenerID<Venta>();
                _repo.Agregar(venta);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar venta: {ex.Message}", ex);
            }
        }

        public void MarcarComoFacturada(int ventaId)
        {
            try
            {
                var lista = _repo.ObtenerTodos().ToList();
                var venta = lista.FirstOrDefault(v => v.ID == ventaId);
                if (venta != null)
                {
                    venta.Estado = "Facturada";
                    _repo.GuardarLista(lista);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al marcar venta como facturada: {ex.Message}", ex);
            }
        }

        // Marca la venta como entregada.
        public void MarcarComoEntregada(int ventaId)
        {
            try
            {
                var lista = _repo.ObtenerTodos().ToList();
                var venta = lista.FirstOrDefault(v => v.ID == ventaId);
                if (venta != null)
                {
                    venta.Estado = "Entregada";
                    _repo.GuardarLista(lista);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al marcar venta como entregada: {ex.Message}", ex);
            }
        }

        // Obtiene ventas entregadas sin comisión asignada.
        public List<Venta> ObtenerVentasSinComisionAsignada()
        {
            try
            {
                var comRepo = new XmlRepository<Comision>("comisiones.xml");
                var idsConCom = comRepo.ObtenerTodos()
                                      .Select(c => c.Venta.ID)
                                      .ToHashSet();

                return _repo.ObtenerTodos()
                            .Where(v => v.Estado == "Entregada" && !idsConCom.Contains(v.ID))
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Guarda la venta finalizada (aprovechando el mismo repositorio).
        public void FinalizarVenta(Venta venta)
        {
            RegistrarVenta(venta);
        }

        // Obtiene todas las ventas con estado "Facturada".
        public List<Venta> ObtenerVentasFacturadas()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Estado == "Facturada")
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Obtiene ventas en un rango de fechas.
        public List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta)
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Fecha.Date >= desde.Date && v.Fecha.Date <= hasta.Date)
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Obtiene todas las ventas con estado "Autorizada".
        public List<Venta> ObtenerVentasAutorizadas()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => string.Equals(v.Estado, "Autorizada", StringComparison.OrdinalIgnoreCase))
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }

        // Obtiene todas las ventas con estado "Entregada".
        public List<Venta> ObtenerVentasEntregadas()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Estado == "Entregada")
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Venta>();
            }
        }
    }
}
