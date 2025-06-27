using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class RegistrarTurnoController
    {
        private readonly TurnoBLL _turnoBll = new();
        private readonly ClienteBLL _clienteBll = new();
        private readonly VehiculoBLL _vehiculoBll = new();

        /// <summary>
        /// Obtiene los vehículos disponibles para agendar.
        /// </summary>
        public List<VehiculoTurnoDto> ObtenerVehiculosParaTurno()
            => _turnoBll.ObtenerVehiculosDisponibles()
                        .Select(VehiculoTurnoDto.FromEntity)
                        .ToList();

        /// <summary>
        /// Valida y registra el turno.
        /// </summary>
        public void RegistrarTurno(TurnoInputDto dto)
        {
            // 1) Cliente existente
            var cliente = _clienteBll.BuscarClientePorDNI(dto.DniCliente)
                         ?? throw new ApplicationException("Cliente no encontrado. Regístrelo primero.");

            // 2) Vehículo en stock
            var veh = _vehiculoBll.BuscarVehiculoPorDominio(dto.DominioVehiculo)
                      ?? throw new ApplicationException("Vehículo no encontrado en el stock.");

            // 3) Disponibilidad de horario
            if (!_turnoBll.VerificarDisponibilidad(veh, dto.Fecha, dto.Hora))
                throw new ApplicationException("El turno ya está ocupado para esa fecha y hora.");

            // 4) Arma el turno
            var turno = new Turno
            {
                Cliente = cliente,
                Vehiculo = veh,
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                Asistencia = "Pendiente"
            };

            // 5) Persiste
            _turnoBll.RegistrarTurno(turno);
        }
    }
}
