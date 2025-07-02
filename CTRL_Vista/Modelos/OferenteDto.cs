using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    // DTO para exponer datos de Oferente en la capa de presentación,
    // filtrando únicamente la información necesaria.
    public class OferenteDto
    {
        public int ID { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }

        // Mapea una entidad Oferente a OferenteDto.
        // o = Entidad Oferente a mapear;
        public static OferenteDto FromEntity(Oferente o)
        {
            if (o == null) return null;

            return new OferenteDto
            {
                ID = o.ID,
                Dni = o.Dni,
                Nombre = o.Nombre,
                Apellido = o.Apellido,
                Contacto = o.Contacto
            };
        }
    }
}
