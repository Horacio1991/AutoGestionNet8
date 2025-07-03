using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.CTRL_Vista
{
    // Gestiona el flujo de ventas:
    // - Listar ventas pendientes
    // - Autorizar o rechazar ventas
    // - Listar ventas autorizadas para facturar
    public class VentaController
    {
        private readonly VentaBLL _ventaBll = new();

        // Obtiene las ventas en estado "Pendiente".
        public List<VentaDto> ObtenerVentasPendientes()
        {
            try
            {
                var entidades = _ventaBll.ObtenerVentasConEstadoPendiente();
                return entidades
                    .Select(VentaDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al obtener ventas pendientes: {ex.Message}", ex);
            }
        }

        public bool AutorizarVenta(int ventaId)
        {
            if (ventaId <= 0)
                throw new ArgumentException("ID de venta inválido.", nameof(ventaId));

            try
            {
                return _ventaBll.AutorizarVenta(ventaId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al autorizar venta: {ex.Message}", ex);
            }
        }

        public bool RechazarVenta(int ventaId, string motivo)
        {
            if (ventaId <= 0)
                throw new ArgumentException("ID de venta inválido.", nameof(ventaId));
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Motivo de rechazo requerido.", nameof(motivo));

            try
            {
                return _ventaBll.RechazarVenta(ventaId, motivo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al rechazar venta: {ex.Message}", ex);
            }
        }

        /// Obtiene las ventas autorizadas ("Autorizada") listas para emitir factura.
        public List<VentaDto> ObtenerVentasParaFacturar()
        {
            try
            {
                var autorizadas = _ventaBll.ObtenerVentasAutorizadas();
                return autorizadas
                    .Select(VentaDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al obtener ventas para facturar: {ex.Message}", ex);
            }
        }
    }
}
