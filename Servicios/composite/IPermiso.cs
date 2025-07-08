namespace AutoGestion.Servicios.Composite
{
    public interface IPermiso
    {
        int ID { get; set; }
        string Nombre { get; set; }

        // Agrega un permiso hijo (simple o compuesto).
        void Agregar(IPermiso permiso);

        // Quita un permiso hijo.
        void Quitar(IPermiso permiso);

        // Devuelve la lista de hijos (permiso simple o compuesto).
        List<IPermiso> ObtenerHijos();
    }
}
