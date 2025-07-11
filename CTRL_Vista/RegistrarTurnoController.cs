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

        // trae los vehiculos que estan estado "Disponible".
        public List<VehiculoTurnoDto> ObtenerVehiculosParaTurno()
        {
            try
            {
                var lista = _turnoBll.ObtenerVehiculosDisponibles();
                return lista
                    .Select(VehiculoTurnoDto.FromEntity)
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al obtener vehículos para turno: {ex.Message}", ex);
            }
        }

        // Valida y registra un nuevo turno.
        public void RegistrarTurno(TurnoInputDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.DniCliente))
                throw new ArgumentException("DNI de cliente requerido.", nameof(dto.DniCliente));
            if (string.IsNullOrWhiteSpace(dto.DominioVehiculo))
                throw new ArgumentException("Dominio de vehículo requerido.", nameof(dto.DominioVehiculo));

            try
            {
                // 1) Ver si el cliente existente
                var cliente = _clienteBll.BuscarClientePorDNI(dto.DniCliente)
                              ?? throw new ApplicationException("Cliente no encontrado. Regístrelo primero.");

                // 2) Verificar vehículo en stock
                var vehiculo = _vehiculoBll.BuscarVehiculoPorDominio(dto.DominioVehiculo)
                              ?? throw new ApplicationException("Vehículo no encontrado en el stock.");

                // 3) Verificar disponibilidad de horario
                bool disponible = _turnoBll.VerificarDisponibilidad(vehiculo, dto.Fecha, dto.Hora);
                if (!disponible)
                    throw new ApplicationException("El turno ya está ocupado para esa fecha y hora.");

                // 4) Construir entidad Turno
                var turno = new Turno
                {
                    Cliente = cliente,
                    Vehiculo = vehiculo,
                    Fecha = dto.Fecha,
                    Hora = dto.Hora,
                    Asistencia = "Pendiente"
                };

                // 5) Persistir turno
                _turnoBll.RegistrarTurno(turno);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al registrar turno: {ex.Message}", ex);
            }
        }
    }
}
