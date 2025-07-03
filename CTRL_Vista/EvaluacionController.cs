using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.CTRL_Vista
{
    /// se usa para la gestión de evaluaciones técnicas de ofertas.
    public class EvaluacionController
    {
        private readonly OfertaBLL _ofertaBll = new();
        private readonly EvaluacionBLL _evalBll = new();

        // Obtiene las ofertas pendientes de inspección
        public List<OfertaListDto> ObtenerOfertasParaEvaluar()
        {
            try
            {
                // 1) Leer ofertas con inspección registrada
                var entidades = _ofertaBll.ObtenerOfertasConInspeccion();

                // 2) Mapear a DTOs y ordenar
                return entidades
                    .Select(OfertaListDto.FromEntity)
                    .OrderBy(dto => dto.FechaInspeccion)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener ofertas para evaluar: {ex.Message}", ex);
            }
        }

        // Registra una evaluación técnica para una oferta existente.
        // dto = DTO con datos de evaluación: OfertaID, EstadoMotor, EstadoCarroceria, etc.
        public void RegistrarEvaluacion(EvaluacionInputDto dto)
        {
            try
            {
                // 1) Validar dto
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                // 2) Buscar la oferta correspondiente
                var oferta = _ofertaBll.ObtenerOfertasConInspeccion()
                                       .FirstOrDefault(o => o.ID == dto.OfertaID)
                            ?? throw new ApplicationException("Oferta no encontrada.");

                // 3) Construir la entidad de EvaluacionTecnica
                var entEval = new Entidades.EvaluacionTecnica
                {
                    ID = GeneradorID.ObtenerID<Entidades.EvaluacionTecnica>(),
                    EstadoMotor = dto.EstadoMotor,
                    EstadoCarroceria = dto.EstadoCarroceria,
                    EstadoInterior = dto.EstadoInterior,
                    EstadoDocumentacion = dto.EstadoDocumentacion,
                    Observaciones = dto.Observaciones
                };

                // 4) Guardar evaluación (BLL)
                _evalBll.GuardarEvaluacion(oferta, entEval);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar evaluación: {ex.Message}", ex);
            }
        }
    }
}
