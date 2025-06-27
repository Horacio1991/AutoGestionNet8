using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class TurnoBLL
    {
        private readonly XmlRepository<Turno> _turnoRepo = new("turnos.xml");
        private readonly XmlRepository<Vehiculo> _vehiculoRepo = new("vehiculos.xml");

        /// <summary>
        /// Vehículos en stock con estado "Disponible".
        /// </summary>
        public List<Vehiculo> ObtenerVehiculosDisponibles()
            => _vehiculoRepo.ObtenerTodos()
                            .Where(v => v.Estado == "Disponible")
                            .ToList();

        /// <summary>
        /// Verifica que no exista ya un turno para ese vehículo en fecha+hora.
        /// </summary>
        public bool VerificarDisponibilidad(Vehiculo vehiculo, DateTime fecha, TimeSpan hora)
            => !_turnoRepo.ObtenerTodos().Any(t =>
                   t.Vehiculo.ID == vehiculo.ID &&
                   t.Fecha.Date == fecha.Date &&
                   t.Hora == hora);

        /// <summary>
        /// Registra el turno y le asigna un nuevo ID.
        /// </summary>
        public Turno RegistrarTurno(Turno turno)
        {
            turno.ID = GeneradorID.ObtenerID<Turno>();
            _turnoRepo.Agregar(turno);
            return turno;
        }

        /// <summary>Obtiene los turnos cuya fecha ya pasó y aún no tienen asistencia registrada.</summary>
        public List<Turno> ObtenerTurnosCumplidos()
        {
            return _turnoRepo.ObtenerTodos()
                        .Where(t => t.Asistencia == "Pendiente")
                        .ToList();
        }

        /// <summary>Registra la asistencia (Asistió/No asistió/Pendiente) y guarda observaciones.</summary>
        public void RegistrarAsistencia(int turnoId, string estado, string observaciones)
        {
            var todos = _turnoRepo.ObtenerTodos();
            var turno = todos.FirstOrDefault(t => t.ID == turnoId);
            if (turno == null)
                throw new ApplicationException("Turno no encontrado.");

            turno.Asistencia = estado;
            turno.Observaciones = observaciones;

            _turnoRepo.GuardarLista(todos);
        }
    }
}
