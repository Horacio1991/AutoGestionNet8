using AutoGestion.BLL;
using AutoGestion.DTOs;

namespace AutoGestion.CTRL_Vista
{
    // Se usa para:
    // - Listar turnos cumplidos pendientes
    // - Registrar asistencia y observaciones

    public class RegistrarAsistenciaController
    {
        private readonly TurnoBLL _turnoBll = new();

        // Obtiene los turnos cuya fecha ya pasó y no tienen asistencia registrada.
        public List<TurnoAsistenciaListDto> ObtenerTurnosParaAsistencia()
        {
            try
            {
                var turnos = _turnoBll.ObtenerTurnosCumplidos();
                return turnos
                    .Select(TurnoAsistenciaListDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener turnos para asistencia: {ex.Message}", ex);
            }
        }

        // Registra la asistencia de un turno específico.
        // dto = DTO con datos de asistencia: TurnoID, Estado y Observaciones.
        public void RegistrarAsistencia(RegistrarAsistenciaInputDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));
                if (dto.TurnoID <= 0)
                    throw new ArgumentException("ID de turno inválido.", nameof(dto.TurnoID));
                if (string.IsNullOrWhiteSpace(dto.Estado))
                    throw new ArgumentException("Estado de asistencia requerido.", nameof(dto.Estado));

                _turnoBll.RegistrarAsistencia(dto.TurnoID, dto.Estado, dto.Observaciones);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar asistencia: {ex.Message}", ex);
            }
        }
    }
}
