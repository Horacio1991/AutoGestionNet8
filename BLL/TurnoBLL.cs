using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class TurnoBLL
    {
        private readonly XmlRepository<Turno> _turnoRepo;
        private readonly XmlRepository<Vehiculo> _vehiculoRepo;

        // 1) Inicializa los repositorios apuntando a "DatosXML/turnos.xml" y "DatosXML/vehiculos.xml".
        public TurnoBLL()
        {
            _turnoRepo = new XmlRepository<Turno>("turnos.xml");
            _vehiculoRepo = new XmlRepository<Vehiculo>("vehiculos.xml");
        }

        // Obtiene la lista de vehículos con estado "Disponible" para agendar turnos.
        public List<Vehiculo> ObtenerVehiculosDisponibles()
        {
            try
            {
                var todos = _vehiculoRepo.ObtenerTodos();
                return todos.Where(v => v.Estado == VehiculoEstados.Disponible)
                           .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Vehiculo>();
            }
        }

        // Verifica si un vehículo está libre en la fecha y hora indicadas.
        public bool VerificarDisponibilidad(Vehiculo vehiculo, DateTime fecha, TimeSpan hora)
        {
            try
            {
                // 1) Leer todos los turnos
                var turnos = _turnoRepo.ObtenerTodos();
                // 2) Comprobar que no haya turnos con el mismo vehículo, fecha y hora
                return !turnos.Any(t =>
                    t.Vehiculo.ID == vehiculo.ID &&
                    t.Fecha.Date == fecha.Date &&
                    t.Hora == hora
                );
            }
            catch (ApplicationException)
            {
                return false;
            }
        }

        public Turno RegistrarTurno(Turno turno)
        {
            try
            {
                turno.ID = GeneradorID.ObtenerID<Turno>();
                _turnoRepo.Agregar(turno);
                return turno;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar turno: {ex.Message}", ex);
            }
        }

        // Obtiene los turnos cuya fecha ya pasó y no tienen asistencia registrada.
        public List<Turno> ObtenerTurnosCumplidos()
        {
            try
            {
                var turnos = _turnoRepo.ObtenerTodos();
                return turnos.Where(t => t.Asistencia == "Pendiente")
                             .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Turno>();
            }
        }

        // Registra la asistencia de un turno existente (Asistió/No asistió/Pendiente)

        public void RegistrarAsistencia(int turnoId, string estado, string observaciones)
        {
            try
            {
                var lista = _turnoRepo.ObtenerTodos();
                var turno = lista.FirstOrDefault(t => t.ID == turnoId)
                            ?? throw new ApplicationException("Turno no encontrado.");
                // Actualizar estado y observaciones
                turno.Asistencia = estado;
                turno.Observaciones = observaciones;
                // Guardar cambios
                _turnoRepo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                throw new ApplicationException($"Error al registrar asistencia: {ex.Message}", ex);
            }
        }
    }
}
