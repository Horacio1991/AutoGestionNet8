using AutoGestion.BLL;
using AutoGestion.DTOs;


namespace AutoGestion.CTRL_Vista
{
    public class RegistrarAsistenciaController
    {
        private readonly TurnoBLL _turnoBll = new();

        /// <summary>Trae los turnos vencidos pendientes de registrar asistencia.</summary>
        public List<TurnoAsistenciaListDto> ObtenerTurnosParaAsistencia()
            => _turnoBll.ObtenerTurnosCumplidos()
                        .Select(t => TurnoAsistenciaListDto.FromEntity(t))
                        .ToList();

        /// <summary>Registra la asistencia del turno seleccionado.</summary>
        public void RegistrarAsistencia(RegistrarAsistenciaInputDto dto)
            => _turnoBll.RegistrarAsistencia(dto.TurnoID, dto.Estado, dto.Observaciones);
    }
}
