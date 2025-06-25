using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class FacturaController
    {
        private readonly FacturaBLL _facturaBLL = new();
        private readonly VentaBLL _ventaBLL = new();

        // Devuelve los DTOs listos para bindear
        public List<VentaDto> ObtenerVentasParaFacturar()
        {
            return new VentaController().ObtenerVentasParaFacturar();
        }

        // Emite la factura y marca la venta como “Facturada”
        public Factura EmitirFactura(int ventaId)
        {
            // Creamos la entidad de Factura
            var venta = _ventaBLL.ObtenerTodas()
                                 .FirstOrDefault(v => v.ID == ventaId);
            if (venta == null)
                throw new ApplicationException("Venta no encontrada");

            var factura = new Factura
            {
                Cliente = venta.Cliente,
                Vehiculo = venta.Vehiculo,
                Precio = venta.Total,
                Fecha = DateTime.Now,
                FormaPago = venta.Pago?.TipoPago ?? "Desconocido"
            };

            // Persistir
            factura = _facturaBLL.EmitirFactura(factura);
            _ventaBLL.MarcarComoFacturada(ventaId);

            return factura;
        }
    }
}
