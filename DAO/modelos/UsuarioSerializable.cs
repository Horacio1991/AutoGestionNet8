namespace AutoGestion.DAO.Modelos
{
    public class UsuarioSerializable
    {
        public int ID { get; set; }
        // Usuario (Login)
        public string Nombre { get; set; }
        // Contraseña (Encriptada)
        public string Clave { get; set; }
        // Rol asignado al usuario
        public string RolNombre { get; set; } 
        public List<string> Permisos { get; set; } = new(); // nombres de permisos asignados directamente
    }
}
