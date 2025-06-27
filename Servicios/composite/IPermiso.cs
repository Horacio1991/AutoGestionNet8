namespace AutoGestion.Servicios.Composite

{
    public interface IPermiso
    {
        int ID { get; set; }
        string Nombre { get; set; }
        void Agregar(IPermiso permiso);
        void Quitar(IPermiso permiso);
        List<IPermiso> ObtenerHijos();
    }

}
