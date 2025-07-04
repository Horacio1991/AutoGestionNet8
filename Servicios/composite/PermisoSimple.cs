using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    // Permiso "Hoja" = no tiene hijos, es un permiso simple. (Por ej, "Emitir Factura")
    [Serializable]
    public class PermisoSimple : IPermiso
    {
        public PermisoSimple() { }

        [XmlElement]
        public int ID { get; set; }

        [XmlElement]
        public string Nombre { get; set; }

        public void Agregar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden agregar hijos a un PermisoSimple.");

        public void Quitar(IPermiso permiso) =>
            throw new InvalidOperationException("No se pueden quitar hijos de un PermisoSimple.");

        public List<IPermiso> ObtenerHijos() => new List<IPermiso>(); //En este caso siempre retorna una lista vacía, ya que no tiene hijos.
    }
}
