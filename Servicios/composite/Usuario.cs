using System.Collections.Generic;


namespace AutoGestion.Servicios.Composite
{
    public class Usuario
    {
        public int ID { get; set; }
        public required string Nombre { get; set; }
        public required string Clave { get; set; } // Encriptado
        public IPermiso Rol { get; set; } // Composite Pattern

        public List<PermisoCompleto> PermisosIndividuales { get; set; } = new();
    }
}
