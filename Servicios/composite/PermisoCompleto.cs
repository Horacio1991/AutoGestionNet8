using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    [Serializable]
    public class PermisoCompleto : IPermiso
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        [XmlArray("Menus")]
        [XmlArrayItem("MenuPermiso")]
        public List<MenuPermiso> MenuItems { get; set; } = new();

        public void Agregar(IPermiso permiso) { }
        public void Quitar(IPermiso permiso) { }
        public List<IPermiso> ObtenerHijos() => new();
    }
}
