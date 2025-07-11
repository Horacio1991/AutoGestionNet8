using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    public class TurnoAsistenciaListDto
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }

        public string Asistencia { get; set; } // Pendiente / Asistió / No asistió


        // Mapea una entidad Turno a TurnoAsistenciaListDto para la UI.
        // t = Entidad Turno a mapear;
        public static TurnoAsistenciaListDto FromEntity(Turno t)
        {
            if (t == null) return null;

            return new TurnoAsistenciaListDto
            {
                ID = t.ID,
                Cliente = $"{t.Cliente?.Nombre} {t.Cliente?.Apellido}".Trim(),
                Vehiculo = $"{t.Vehiculo?.Marca} {t.Vehiculo?.Modelo} ({t.Vehiculo?.Dominio})",
                Fecha = t.Fecha.ToShortDateString(),
                Hora = t.Hora.ToString(@"hh\:mm"),
                Asistencia = string.IsNullOrWhiteSpace(t.Asistencia)
                                ? "Pendiente"
                                : t.Asistencia
            };
        }
    }
}
