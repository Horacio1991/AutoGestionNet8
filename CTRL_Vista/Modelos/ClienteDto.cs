namespace AutoGestion.CTRL_Vista.Modelos
{
    // DTO para exponer sólo los datos necesarios al UI
    public class ClienteDto
    {
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }

        // Mapeo desde la entidad de dominio
        public static ClienteDto FromEntity(Entidades.Cliente c) => new ClienteDto
        {
            Dni = c.Dni,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Contacto = c.Contacto
        };
    }
}
