using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;


namespace AutoGestion.CTRL_Vista
{
    public class VentaController
    {
        private readonly VentaBLL _ventaBLL = new();

        // 1) Obtener todos los pendientes (estado "Pendiente") ya mapeados a DTO
        public List<VentaDto> ObtenerVentasPendientes()
        {
            return _ventaBLL.ObtenerVentasConEstadoPendiente()
                   .Select(VentaDto.FromEntity)
                   .ToList();
        }

        // 2) Autorizar una venta por ID
        public bool AutorizarVenta(int id) => _ventaBLL.AutorizarVenta(id);

        // 3) Rechazar una venta por ID con motivo
        public bool RechazarVenta(int id, string motivo) =>
            _ventaBLL.RechazarVenta(id, motivo);

        // 4) Obtener las ventas autorizadas listas para facturar
        public List<VentaDto> ObtenerVentasParaFacturar()
        {
            var entidades = _ventaBLL.ObtenerVentasPendientes()   // Todas las no facturadas
                                     .Where(v => v.Estado == "Autorizada")
                                     .ToList();

            return entidades.Select(VentaDto.FromEntity)
                            .ToList();
        }
    }
}
