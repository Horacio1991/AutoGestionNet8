namespace AutoGestion.DTOs
{
    public class OferenteDto
    {
        public int ID { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }

        public static OferenteDto FromEntity(Entidades.Oferente o) => new()
        {
            ID = o.ID,
            Dni = o.Dni,
            Nombre = o.Nombre,
            Apellido = o.Apellido,
            Contacto = o.Contacto
        };
    }
}
