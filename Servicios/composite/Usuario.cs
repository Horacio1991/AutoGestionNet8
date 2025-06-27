namespace AutoGestion.Servicios.Composite
{
    public class Usuario
    {
        public int ID { get; set; }
        public required string Nombre { get; set; }
        public required string Clave { get; set; }    // Encriptada
        public IPermiso? Rol { get; set; }            // Rol compuesto con permisos
    }
}
