namespace AutoGestion.Servicios.Composite
{
    // Interfaz común para todos los permisos.
    public interface IPermiso
    {
        int ID { get; set; }
        string Nombre { get; set; }
        void Agregar(IPermiso permiso);
        void Quitar(IPermiso permiso);

        // lista de hijos directos.
        List<IPermiso> ObtenerHijos();
    }
}
