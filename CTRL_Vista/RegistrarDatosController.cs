using AutoGestion.BLL;
using AutoGestion.DTOs;

namespace AutoGestion.CTRL_Vista
{
    // se usa para ya registrar la compra de un vehiculo
    // tras haber completado la evaluación técnica.
    public class RegistrarDatosController
    {
        private readonly OfertaBLL _ofertaBll = new();
        private readonly EvaluacionBLL _evaluacionBll = new();
        private readonly VehiculoBLL _vehiculoBll = new();

        // Obtiene los datos necesarios para mostrar la oferta y evaluación previa,
        // antes de registrar el estado de stock.
        public OfertaRegistroDto ObtenerOfertaPorDominio(string dominio)
        {
            if (string.IsNullOrWhiteSpace(dominio))
                throw new ArgumentException("Dominio requerido.", nameof(dominio));

            try
            {
                // 1) Buscar oferta sin procesar por dominio
                var oferta = _ofertaBll.ObtenerOfertasSinRegistrar()
                    .FirstOrDefault(o =>
                        o.Vehiculo.Dominio
                         .Equals(dominio, StringComparison.OrdinalIgnoreCase));

                if (oferta == null)
                    return null;

                // Obtener evaluación asociada
                var evaluacion = _evaluacionBll.ObtenerEvaluacionAsociada(oferta);

                if (evaluacion == null)
                    return null;

                // construir y devolver el DTO
                return new OfertaRegistroDto
                {
                    OfertaID = oferta.ID,
                    EvaluacionTexto =
                        $"Motor: {evaluacion.EstadoMotor}\r\n" +
                        $"Carrocería: {evaluacion.EstadoCarroceria}\r\n" +
                        $"Interior: {evaluacion.EstadoInterior}\r\n" +
                        $"Documentación: {evaluacion.EstadoDocumentacion}"
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener datos de registro: {ex.Message}", ex);
            }
        }


        // Registra el estado de stock final del vehículo y marca la oferta como procesada.
        public void RegistrarDatos(RegistrarDatosInputDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (dto.OfertaID <= 0)
                throw new ArgumentException("OfertaID inválido.", nameof(dto.OfertaID));
            if (string.IsNullOrWhiteSpace(dto.EstadoStock))
                throw new ArgumentException("Estado de stock requerido.", nameof(dto.EstadoStock));

            try
            {
                // 1) trae ofertas sin registrar y busca la oferta por ID:
                var oferta = _ofertaBll.ObtenerOfertasSinRegistrar()
                    .FirstOrDefault(o => o.ID == dto.OfertaID);
                if (oferta == null)
                    throw new ApplicationException("Oferta no encontrada.");

                // 2) Gestionar stock del vehículo:
                //    - Si ya existe en vehiculos.xml, actualizar su estado.
                //    - Si no existe y estado es "Disponible", agregar al stock.
                var dominio = oferta.Vehiculo.Dominio;
                var existente = _vehiculoBll.BuscarVehiculoPorDominio(dominio);
                if (existente != null)
                {
                    _vehiculoBll.ActualizarEstadoVehiculo(existente, dto.EstadoStock);
                }
                else if (dto.EstadoStock.Equals("Disponible", StringComparison.OrdinalIgnoreCase))
                {
                    _vehiculoBll.AgregarVehiculoAlStock(oferta.Vehiculo);
                }

                // 3) Marcar la oferta como procesada
                oferta.Estado = "Registrada";
                _ofertaBll.ActualizarOferta(oferta);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al registrar datos de oferta: {ex.Message}", ex);
            }
        }

 
    }
}
