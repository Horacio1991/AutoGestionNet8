using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class PagoController
    {
        private readonly ClienteBLL _clienteBll = new();
        private readonly VehiculoBLL _vehiculoBll = new();
        private readonly PagoBLL _pagoBll = new();
        private readonly VentaBLL _ventaBll = new();

        // Recupera todos los vehículos con estado "Disponible" y los mapea a DTOs.
        public List<VehiculoDto> ObtenerVehiculosDisponibles()
        {
            try
            {
                var entidades = _vehiculoBll.ObtenerDisponibles();
                return entidades
                    .Select(VehiculoDto.FromEntity)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener vehículos disponibles: {ex.Message}", ex);
            }
        }

        // Busca un Cliente por su DNI.  
        public ClienteDto BuscarCliente(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni))
                    throw new ArgumentException("DNI de cliente requerido.", nameof(dni));

                var entidad = _clienteBll.BuscarClientePorDNI(dni);
                return entidad is null
                    ? null
                    : ClienteDto.FromEntity(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar cliente: {ex.Message}", ex);
            }
        }

        // Registra el pago y crea la venta en estado "Pendiente". 
        // Devuelve true si todo fue exitoso; en caso contrario, devuelve false y la descripción del error en out.
        public bool RegistrarPagoYVenta(
            string clienteDni,
            string vehiculoDominio,
            string tipoPago,
            decimal monto,
            int cuotas,
            string detalles,
            int vendedorId,
            string vendedorNombre,
            out string error)
        {
            error = null;
            try
            {
                // 1) Validaciones básicas
                if (string.IsNullOrWhiteSpace(clienteDni))
                    throw new ApplicationException("DNI de cliente requerido.");
                if (string.IsNullOrWhiteSpace(vehiculoDominio))
                    throw new ApplicationException("Dominio de vehículo requerido.");
                if (monto <= 0)
                    throw new ApplicationException("El monto debe ser mayor que cero.");

                // 2) Crear y persistir Pago
                var pago = new Pago
                {
                    TipoPago = tipoPago,
                    Monto = monto,
                    Cuotas = cuotas,
                    Detalles = detalles,
                    FechaPago = DateTime.Now
                };
                _pagoBll.RegistrarPago(pago);

                // 3) Recuperar Cliente y Vehículo
                var cliente = _clienteBll
                    .BuscarClientePorDNI(clienteDni)
                    ?? throw new ApplicationException("Cliente no encontrado.");

                var vehiculo = _vehiculoBll
                    .BuscarVehiculoPorDominio(vehiculoDominio)
                    ?? throw new ApplicationException("Vehículo no encontrado.");

                // 4) Armar y persistir Venta pendiente
                var venta = new Venta
                {
                    Cliente = cliente,
                    Vehiculo = vehiculo,
                    Pago = pago,
                    Estado = "Pendiente",
                    Fecha = DateTime.Now,
                    Vendedor = new Vendedor { ID = vendedorId, Nombre = vendedorNombre }
                };
                _ventaBll.FinalizarVenta(venta);

                // 5) Cambiar estado del vehículo a “En Proceso”
                _vehiculoBll.ActualizarEstadoVehiculo(vehiculo, "En Proceso");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
