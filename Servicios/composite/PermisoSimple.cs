namespace AutoGestion.Servicios.Composite

{
    public class PermisoSimple : IPermiso
    {
        public string Nombre { get; set; }
        //Como es hoja, no tiene hijos, por lo que no se implementa la lógica de agregar o quitar permisos.
        public void Agregar(IPermiso permiso) { }

        public void Quitar(IPermiso permiso) { }
        // Retorna una lista vacía ya que no tiene hijos.
        public List<IPermiso> ObtenerHijos() => new();
    }
}
