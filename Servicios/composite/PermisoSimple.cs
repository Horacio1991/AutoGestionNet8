namespace AutoGestion.Servicios.Composite

{
    /// <summary>
    /// Permiso atómico (leaf). No tiene hijos.
    /// </summary>
    [Serializable]
    public class PermisoSimple : IPermiso
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        // Estas operaciones no aplican en un leaf
        public void Agregar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden agregar permisos a un PermisoSimple.");

        public void Quitar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden quitar permisos de un PermisoSimple.");

        public List<IPermiso> ObtenerHijos() => new List<IPermiso>();
    }

}
