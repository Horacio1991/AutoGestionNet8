using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    // Usado en el proceso de entrega de vehículos:
    // listados de ventas facturadas y confirmación de entrega.
    public class EntregaController
    {
        private readonly VentaBLL _ventaBll = new();

        // Obtiene las ventas cuyo estado es "Facturada",
        public List<VentaDto> ObtenerVentasParaEntrega()
        {
            try
            {
                // 1) Traer todas las ventas con estado Facturada
                var ventas = _ventaBll.ObtenerVentasFacturadas();

                // 2) Mapear cada entidad Venta a VentaDto
                return ventas
                    .Select(VentaDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ventas para entrega: {ex.Message}", ex);
            }
        }

        // Marca una venta como entregada.
        public void ConfirmarEntrega(int ventaId)
        {
            try
            {
                // 1) Verificar existencia
                var venta = _ventaBll.ObtenerTodas().FirstOrDefault(v => v.ID == ventaId)
                            ?? throw new ApplicationException("Venta no encontrada.");

                // 2) marcar como entregada
                _ventaBll.MarcarComoEntregada(ventaId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al confirmar entrega: {ex.Message}", ex);
            }
        }

        // Recupera la entidad completa de venta para generar el comprobante PDF.
        public Venta ObtenerEntidad(int ventaId)
        {
            try
            {
                var venta = _ventaBll.ObtenerTodas().FirstOrDefault(v => v.ID == ventaId)
                            ?? throw new ApplicationException("Venta no encontrada.");

                return venta;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al recuperar entidad de venta: {ex.Message}", ex);
            }
        }
    }
}
