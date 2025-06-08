using System.Collections.Generic;

namespace AutoGestion.DAO.Modelos
{
    public class UsuarioSerializable
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }

        public string RolNombre { get; set; }  // solo el nombre del rol
        public List<string> Permisos { get; set; } = new(); // nombres de permisos asignados directamente
    }
}
