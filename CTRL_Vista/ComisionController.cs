using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    //  Es usado para toda la lógica relacionada con comisiones:
    // - Listar ventas sin comisión
    // - Aprobar/rechazar comisiones
    // - Consultar comisiones
    public class ComisionController
    {
        private readonly ComisionBLL _comisionBll = new();
        private readonly VentaBLL _ventaBll = new();

        // Obtiene las ventas que ya fueron entregadas pero aún no tienen comisión.
        public List<VentaComisionDto> ObtenerVentasSinComision()
        {
            try
            {
                // 1) Traer entidades Venta
                var ventas = _comisionBll.ObtenerVentasSinComision();

                // 2) Mapear a DTOs de presentación, incluyendo comisión sugerida al 5%
                return ventas.Select(v => new VentaComisionDto // Select Transforma Venta a DTO
                {
                    VentaID = v.ID,
                    Cliente = $"{v.Cliente.Nombre} {v.Cliente.Apellido}".Trim(),
                    Vendedor = v.Vendedor?.Nombre ?? "N/D",
                    VehiculoResumen = $"{v.Vehiculo.Marca} {v.Vehiculo.Modelo} ({v.Vehiculo.Dominio})",
                    MontoVenta = v.Total,
                    ComisionSugerida = Math.Round(v.Total * 0.05m, 2),
                    FechaVenta = v.Fecha.ToShortDateString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ventas sin comisión: {ex.Message}", ex);
            }
        }

        // Registra una comisión, ya sea aprobada o rechazada.
        // dto = DTO con datos de comisión: VentaID, Monto, Estado y MotivoRechazo (opcional).
        public bool RegistrarComision(ComisionInputDto dto)
        {
            try
            {
                // 1) Validar entrada mínima
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));
                if (dto.VentaID <= 0)
                    throw new ArgumentException("VentaID inválido.", nameof(dto.VentaID));
                if (string.IsNullOrWhiteSpace(dto.Estado))
                    throw new ArgumentException("Estado de comisión requerido.", nameof(dto.Estado));
                if (dto.Estado.Equals("Rechazada", StringComparison.OrdinalIgnoreCase)
                    && string.IsNullOrWhiteSpace(dto.MotivoRechazo))
                    throw new ArgumentException("Motivo de rechazo requerido para comisiones rechazadas.", nameof(dto.MotivoRechazo));

                // 2) Obtener la venta completa
                var venta = _ventaBll.ObtenerDetalleVenta(dto.VentaID)
                            ?? throw new ApplicationException("Venta no encontrada.");

                // 3) Construir entidad Comision
                var comision = new Comision
                {
                    Venta = venta,
                    Porcentaje = venta.Total > 0 ? dto.Monto / venta.Total : 0m,
                    Monto = dto.Monto,
                    Estado = dto.Estado,
                    MotivoRechazo = dto.Estado.Equals("Rechazada", StringComparison.OrdinalIgnoreCase)
                                        ? dto.MotivoRechazo
                                        : null
                };

                // 4) Delegar a la BLL
                return _comisionBll.RegistrarComision(comision);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar comisión: {ex.Message}", ex);
            }
        }

        // Obtiene comisiones de un vendedor según estado y rango de fechas.
        public List<ComisionListDto> ObtenerComisiones(
            int vendedorId,
            string estado,
            DateTime desde,
            DateTime hasta)
        {
            try
            {
                // 1) Traer entidades Comision filtradas
                var entidades = _comisionBll.ObtenerComisionesPorVendedorYFiltros(
                    vendedorId, estado, desde, hasta);

                // 2) Mapear a DTOs listos para UI
                return entidades
                    .Select(ComisionListDto.FromEntity)
                    .Where(d => d != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener comisiones: {ex.Message}", ex);
            }
        }
    }
}
