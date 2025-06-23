using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    [Serializable]
    // Representa un permiso completo que contiene un ID, un nombre y una lista de permisos de menú.
    public class PermisoCompleto : IPermiso
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        // Lista de permisos de menú asociados a este permiso completo.
        // Cada elemento indica un menu y las acciones permitidas dentro de ese menú.
        [XmlArray("Menus")]
        [XmlArrayItem("MenuPermiso")]
        public List<MenuPermiso> MenuItems { get; set; } = new();

        // No aplican para este tipo de permiso, ya que es un permiso completo y no un contenedor de permisos.
        public void Agregar(IPermiso permiso) { }
        public void Quitar(IPermiso permiso) { }
        public List<IPermiso> ObtenerHijos() => new();
    }
}
