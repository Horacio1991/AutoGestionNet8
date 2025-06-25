using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;
using AutoGestion.BLL;

namespace AutoGestion.CTRL_Vista
{
    public class EntregaController
    {
        private readonly VentaBLL _bll = new();

        // Devuelve DTOs para poblar el grid
        public List<VentaDto> ObtenerVentasParaEntrega()
        {
            return _bll.ObtenerVentasFacturadas()
                       .Select(VentaDto.FromEntity)
                       .ToList();
        }

        // Marca la venta como entregada
        public void ConfirmarEntrega(int ventaId)
        {
            _bll.MarcarComoEntregada(ventaId);
        }

        // Recupera la entidad completa para generar el PDF
        public Venta ObtenerEntidad(int ventaId)
        {
            return _bll.ObtenerTodas()
                       .FirstOrDefault(v => v.ID == ventaId);
        }
    }
}
