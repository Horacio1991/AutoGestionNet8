namespace AutoGestion.Servicios.Composite
{
    //Representa un usuario con su permiso (rol compuesto).
    public class Usuario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; } 

        // debería ser un PermisoCompuesto con toda la jerarquía.
        public IPermiso? Rol { get; set; }
    }
}
