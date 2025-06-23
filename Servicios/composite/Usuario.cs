namespace AutoGestion.Servicios.Composite
{
    public class Usuario
    {
        public int ID { get; set; }
        public required string Nombre { get; set; }
        public required string Clave { get; set; } // Encriptada

        // Rol principal del usuario, segun el patron Composite. Define los permisos que tiene el usuario.
        public IPermiso Rol { get; set; } 

        public List<PermisoCompleto> PermisosIndividuales { get; set; } = new();
    }
}
