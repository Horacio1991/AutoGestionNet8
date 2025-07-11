using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    [Serializable]
    [XmlInclude(typeof(PermisoSimple))]
    [XmlInclude(typeof(PermisoCompuesto))]
    public class PermisoCompuesto : IPermiso
    {
        public PermisoCompuesto() { }

        [XmlElement]
        public int ID { get; set; }

        [XmlElement]
        public string Nombre { get; set; }

        // Sub-permisos compuestos (ej. "Gestión Ventas").
        [XmlArray("HijosCompuestos")]
        [XmlArrayItem("PermisoCompuesto", typeof(PermisoCompuesto))]
        public List<PermisoCompuesto> HijosCompuestos { get; set; } = new();

        // Permisos simples (ej "Solicitar Modelo").
        [XmlArray("HijosSimples")]
        [XmlArrayItem("PermisoSimple", typeof(PermisoSimple))]
        public List<PermisoSimple> HijosSimples { get; set; } = new();

        // Unión de HijosCompuestos y HijosSimples. No se serializa.
        [XmlIgnore]
        public List<IPermiso> Hijos
        {
            get
            {
                var todos = new List<IPermiso>();
                todos.AddRange(HijosCompuestos);
                todos.AddRange(HijosSimples);
                return todos;
            }
        }

        public void Agregar(IPermiso permiso)
        {
            switch (permiso)
            {
                case PermisoCompuesto pc:
                    HijosCompuestos.Add(pc);
                    break;
                case PermisoSimple ps:
                    HijosSimples.Add(ps);
                    break;
                default:
                    throw new ArgumentException("Tipo de permiso no soportado", nameof(permiso));
            }
        }

        public void Quitar(IPermiso permiso)
        {
            switch (permiso)
            {
                case PermisoCompuesto pc:
                    HijosCompuestos.RemoveAll(x => x.ID == pc.ID);
                    break;
                case PermisoSimple ps:
                    HijosSimples.RemoveAll(x => x.ID == ps.ID);
                    break;
            }
        }

        public List<IPermiso> ObtenerHijos() => new List<IPermiso>(Hijos);
    }
}
