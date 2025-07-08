namespace AutoGestion.Servicios.Composite
{
    // Nodo hoja del Composite: no puede tener hijos.
    // Representa un ToolStripMenuItem.
    [Serializable]
    public class PermisoSimple : IPermiso
    {
        public PermisoSimple() { }

        public int ID { get; set; }
        public string Nombre { get; set; }

        public void Agregar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden agregar hijos a un PermisoSimple.");

        public void Quitar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden quitar hijos de un PermisoSimple.");

        public List<IPermiso> ObtenerHijos() => new List<IPermiso>();
    }
}
