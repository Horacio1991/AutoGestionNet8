using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Servicios.Utilidades;
using Ent = AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class EvaluacionController
    {
        private readonly OfertaBLL _ofertaBll = new();
        private readonly EvaluacionBLL _evaluacionBll = new();

        /// <summary> Trae todas las ofertas pendientes de inspección como DTOs. </summary>
        public List<OfertaListDto> ObtenerOfertasParaEvaluar()
        {
            var entidades = _ofertaBll.ObtenerOfertasConInspeccion();
            return entidades.Select(OfertaListDto.FromEntity)
                            .OrderBy(o => o.FechaInspeccion)
                            .ToList();
        }

        /// <summary> Registra una evaluación técnica para una oferta existente. </summary>
        public void RegistrarEvaluacion(EvaluacionInputDto dto)
        {
            // 1) Busco la oferta original
            var oferta = _ofertaBll.ObtenerOfertasConInspeccion()
                                   .FirstOrDefault(o => o.ID == dto.OfertaID);
            if (oferta == null)
                throw new ApplicationException("Oferta no encontrada.");

            // 2) Creo la entidad EvaluacionTecnica
            var entEval = new Ent.EvaluacionTecnica
            {
                ID = GeneradorID.ObtenerID<Ent.EvaluacionTecnica>(),
                EstadoMotor = dto.EstadoMotor,
                EstadoCarroceria = dto.EstadoCarroceria,
                EstadoInterior = dto.EstadoInterior,
                EstadoDocumentacion = dto.EstadoDocumentacion,
                Observaciones = dto.Observaciones
            };

            // 3) La guardo
            _evaluacionBll.GuardarEvaluacion(oferta, entEval);
        }
    }
}
