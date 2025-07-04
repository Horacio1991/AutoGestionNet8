namespace AutoGestion.Servicios.Composite
{
    // Usuario del sistema, con un rol (árbol de permisos) asignado.
    public class Usuario
    {
        public int ID { get; set; }
        public required string Nombre { get; set; }
        public required string Clave { get; set; }    // Encriptada
        public IPermiso? Rol { get; set; }            // Rol compuesto con permisos
    }
}
