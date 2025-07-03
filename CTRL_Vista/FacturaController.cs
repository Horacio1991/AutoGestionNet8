using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class FacturaController
    {
        private readonly FacturaBLL _facturaBll = new();
        private readonly VentaBLL _ventaBll = new();

        // Obtiene las ventas ya autorizadas (que se pueden facturar),
        public List<VentaDto> ObtenerVentasParaFacturar()
        {
            try
            {
                // 1) Todas las ventas con estado "Autorizada"
                var entidades = _ventaBll.ObtenerVentasAutorizadas();

                // 2) Mapear a DTO
                return entidades
                    .Select(VentaDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ventas para facturar: {ex.Message}", ex);
            }
        }

        /// Emite una factura para la venta (ventaId) y marca la venta como “Facturada”.
        public Factura EmitirFactura(int ventaId)
        {
            try
            {
                // 1) Obtener la venta
                var venta = _ventaBll.ObtenerDetalleVenta(ventaId)
                            ?? throw new ApplicationException("Venta no encontrada.");

                // 2) Construir la entidad Factura
                var factura = new Factura
                {
                    Cliente = venta.Cliente,
                    Vehiculo = venta.Vehiculo,
                    Precio = venta.Total,
                    Fecha = DateTime.Now,
                    FormaPago = venta.Pago?.TipoPago ?? "Desconocido"
                };

                // 3) Persistir factura
                var emitida = _facturaBll.EmitirFactura(factura);

                // 4) Marcar la venta como facturada
                _ventaBll.MarcarComoFacturada(ventaId);

                return emitida;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al emitir factura: {ex.Message}", ex);
            }
        }
    }
}
