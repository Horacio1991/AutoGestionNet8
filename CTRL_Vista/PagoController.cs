using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    // Lo uso para gestionar el flujo de pago y venta:
    // - Mostrar vehículos disponibles
    // - Buscar cliente
    // - Registrar pago y venta pendiente
    public class PagoController
    {
        private readonly ClienteBLL _clienteBll = new();
        private readonly VehiculoBLL _vehiculoBll = new();
        private readonly PagoBLL _pagoBll = new();
        private readonly VentaBLL _ventaBll = new();

        // Obtiene los DTO de los vehículos disponibles para venta.
        public List<VehiculoDto> ObtenerVehiculosDisponibles()
        {
            try
            {
                var entidades = _vehiculoBll.ObtenerDisponibles();
                return entidades
                    .Select(VehiculoDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener vehículos disponibles: {ex.Message}", ex);
            }
        }

        // Busca un cliente por DNI. 
        public Cliente BuscarCliente(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni))
                    throw new ArgumentException("DNI de cliente requerido.", nameof(dni));

                return _clienteBll.BuscarClientePorDNI(dni);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar cliente: {ex.Message}", ex);
            }
        }

        // Registra un pago y crea una venta en estado "Pendiente".
        // También actualiza el estado del vehículo a "En Proceso", para evitar que se venda nuevamente.
        public bool RegistrarPagoYVenta(
            string clienteDni,
            string vehiculoDominio,
            string tipoPago,
            decimal monto,
            int cuotas,
            string detalles,
            int vendedorId,
            string vendedorNombre)
        {
            try
            {
                // 1) Validar inputs
                if (string.IsNullOrWhiteSpace(clienteDni))
                    throw new ArgumentException("DNI de cliente requerido.", nameof(clienteDni));
                if (string.IsNullOrWhiteSpace(vehiculoDominio))
                    throw new ArgumentException("Dominio de vehículo requerido.", nameof(vehiculoDominio));
                if (monto <= 0)
                    throw new ArgumentException("Monto debe ser mayor a cero.", nameof(monto));

                // 2) Crear y guardar Pago
                var pago = new Pago
                {
                    TipoPago = tipoPago,
                    Monto = monto,
                    Cuotas = cuotas,
                    Detalles = detalles,
                    FechaPago = DateTime.Now
                };
                _pagoBll.RegistrarPago(pago);

                // 3) Recuperar entidad Cliente
                var cliente = _clienteBll.BuscarClientePorDNI(clienteDni)
                              ?? throw new ApplicationException("Cliente no encontrado.");

                // 4) Recuperar entidad Vehículo
                var vehiculo = _vehiculoBll.BuscarVehiculoPorDominio(vehiculoDominio)
                               ?? throw new ApplicationException("Vehículo no encontrado.");

                // 5) Armar entidad Venta
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

                // 6) Reservar vehículo: cambiar a “En Proceso”
                _vehiculoBll.ActualizarEstadoVehiculo(vehiculo, "En Proceso");

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar pago y venta: {ex.Message}", ex);
            }
        }
    }
}
