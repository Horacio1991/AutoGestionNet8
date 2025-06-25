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

        public List<VehiculoDto> ObtenerVehiculosDisponibles()
        {
            return _vehiculoBll.ObtenerDisponibles()
                              .Select(v => VehiculoDto.FromEntity(v))
                              .ToList();
        }


        public Entidades.Cliente BuscarCliente(string dni)
        {
            // Aquí aún devolvemos la entidad Cliente, o podríamos mapear a un ClienteDto si quisiéramos
            return _clienteBll.BuscarClientePorDNI(dni);
        }

        public bool RegistrarPagoYVenta(VentaAltaDto dto, int vendedorId, string vendedorNombre)
        {
            // 1) Registrar Pago
            var pago = new Pago
            {
                TipoPago = dto.Pago.TipoPago,
                Monto = dto.Pago.Monto,
                Cuotas = dto.Pago.Cuotas,
                Detalles = dto.Pago.Detalles
            };
            _pagoBll.RegistrarPago(pago);

            // 2) Armar y registrar Venta
            var cliente = _clienteBll.BuscarClientePorDNI(dto.ClienteDni);
            var vehiculo = _vehiculoBll.BuscarVehiculoPorDominio(dto.VehiculoDominio);

            var venta = new Venta
            {
                Cliente = cliente,
                Vehiculo = vehiculo,
                Pago = pago,
                Estado = "Pendiente",
                Vendedor = new Vendedor { ID = vendedorId, Nombre = vendedorNombre }
            };

            _ventaBll.FinalizarVenta(venta);
            _vehiculoBll.ActualizarEstadoVehiculo(vehiculo, "En Proceso");

            return true;
        }
    }
}
