using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.BLL
{
    public class TurnoBLL
    {
        // Repositorio que lee/escribe List<Turno> en "DatosXML/turnos.xml"
        private readonly XmlRepository<Turno> _repo = new("turnos.xml");

        public bool EstaDisponible(DateTime fecha, TimeSpan hora, Vehiculo vehiculo)
        {
            // Comprueba si ya existe un turno para la misma fecha, hora y vehículo
            return !_repo.ObtenerTodos().Any(t =>
                t.Fecha.Date == fecha.Date &&
                t.Hora == hora &&
                t.Vehiculo.Dominio == vehiculo.Dominio);
        }

        public void RegistrarTurno(Cliente cliente, Vehiculo vehiculo, DateTime fecha, TimeSpan hora)
        {
            var turno = new Turno
            {
                ID = GeneradorID.ObtenerID<Turno>(),
                Cliente = cliente,
                Vehiculo = vehiculo,
                Fecha = fecha.Date,
                Hora = hora,
                Asistencia = "Pendiente",
                Observaciones = ""
            };

            _repo.Agregar(turno);
        }

        // Devuelve los turnos que ya cumplieron (fecha pasada o hoy con hora pasada)
        // Usado para registrar asistencia
        public List<Turno> ObtenerTurnosCumplidos()
        {
            var lista = _repo.ObtenerTodos();
            return lista.Where(t => t.Fecha.Date < DateTime.Today ||
                                   (t.Fecha.Date == DateTime.Today && t.Hora < DateTime.Now.TimeOfDay)).ToList();
        }

        public void RegistrarAsistencia(int turnoId, string estado, string observaciones)
        {
            var lista = _repo.ObtenerTodos();
            var turno = lista.FirstOrDefault(t => t.ID == turnoId);
            if (turno != null)
            {
                turno.Asistencia = estado;
                turno.Observaciones = observaciones;
                _repo.GuardarLista(lista);
            }
        }

    }
}
