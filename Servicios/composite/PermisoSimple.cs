using System.Collections.Generic;

namespace AutoGestion.Servicios.Composite

{
    public class PermisoSimple : IPermiso
    {
        public string Nombre { get; set; }

        public void Agregar(IPermiso permiso) { }

        public void Quitar(IPermiso permiso) { }

        public List<IPermiso> ObtenerHijos() => new();
    }
}
