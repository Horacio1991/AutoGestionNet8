using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista.Modelos
{
    public class ClienteDto
    {
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }

        // Mapea una entidad Cliente a ClienteDTO.
        // c = Entidad Cliente a mapear;
        // ClienteDTO con sólo los campos necesarios para la UI;
        public static ClienteDto FromEntity(Cliente c)
        {
            // 1) Validar que la entidad no sea null para evitar NullReferenceException en la vista
            if (c == null)
                return null;

            // 2) Mapear únicamente los campos que la capa de presentación requiere
            return new ClienteDto
            {
                Dni = c.Dni,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Contacto = c.Contacto
            };
        }
    }
}
