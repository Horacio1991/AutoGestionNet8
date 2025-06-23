namespace AutoGestion.Servicios.Composite

{
    public interface IPermiso
    {
        string Nombre { get; set; }

        void Agregar(IPermiso permiso);
        void Quitar(IPermiso permiso);
        List<IPermiso> ObtenerHijos();
    }
}
