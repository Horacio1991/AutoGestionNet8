// AutoGestion.DTOs/TurnoAsistenciaListDto.cs
namespace AutoGestion.DTOs
{
    public class TurnoAsistenciaListDto
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Asistencia { get; set; }

        public static TurnoAsistenciaListDto FromEntity(AutoGestion.Entidades.Turno t)
            => new()
            {
                ID = t.ID,
                Cliente = $"{t.Cliente.Nombre} {t.Cliente.Apellido}",
                Vehiculo = $"{t.Vehiculo.Marca} {t.Vehiculo.Modelo} ({t.Vehiculo.Dominio})",
                Fecha = t.Fecha.ToShortDateString(),
                Hora = t.Hora.ToString(@"hh\:mm"),
                Asistencia = t.Asistencia ?? "Pendiente"
            };
    }
}


