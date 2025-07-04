using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    // Representa un permiso que puede agrupar otros permisos:
    //  - PermisoSimple (acciones individuales, p.ej. “Emitir Factura”)
    //  - PermisoCompuesto (submenús o plantillas completas)

    [Serializable] //La clase debe ser serializable para XML.
    [XmlInclude(typeof(PermisoSimple))] // Permite serializar PermisoSimple dentro de PermisoCompuesto.
    [XmlInclude(typeof(PermisoCompuesto))] // Permite serializar PermisoCompuesto dentro de PermisoCompuesto.
    public class PermisoCompuesto : IPermiso
    {
        [XmlElement] //Se usa para tratar la propiedad como un elemento XML.
        public int ID { get; set; }

        [XmlElement]
        public string Nombre { get; set; }

        // Sub-permisos compuestos (p.ej. submenús o plantillas anidadas).
        [XmlArray("HijosCompuestos")] // Se usa para serializar esta lista como un elemento XML.
        [XmlArrayItem("PermisoCompuesto", typeof(PermisoCompuesto))] // Elementos de tipo PermisoCompuesto dentro de la lista.
        public List<PermisoCompuesto> HijosCompuestos { get; set; } = new();

        // Permisos simples (acciones puntuales dentro de un menú).
        // Se usa para asignar ítems concretos que el usuario podrá ejecutar.
        [XmlArray("HijosSimples")]
        [XmlArrayItem("PermisoSimple", typeof(PermisoSimple))]
        public List<PermisoSimple> HijosSimples { get; set; } = new();

        // Acceso unificado a todos los hijos (compuestos + simples),
        // sirve para recorrer el árbol sin distinguir tipos.
        [XmlIgnore] // No se serializa, es una propiedad calculada.
        public List<IPermiso> Hijos =>
            HijosCompuestos.Cast<IPermiso>()
                           .Concat(HijosSimples)
                           .ToList();

        // Agrega un permiso hijo:
        //  - Si es compuesto, lo añade a HijosCompuestos
        //  - Si es simple, lo añade a HijosSimples
        public void Agregar(IPermiso permiso)
        {
            switch (permiso)
            {
                case PermisoCompuesto pc:
                    if (!HijosCompuestos.Any(x => x.ID == pc.ID))
                        HijosCompuestos.Add(pc);
                    break;

                case PermisoSimple ps:
                    if (!HijosSimples.Any(x => x.ID == ps.ID))
                        HijosSimples.Add(ps);
                    break;

                default:
                    throw new ArgumentException("Tipo de permiso no soportado", nameof(permiso));
            }
        }

        // Se aplica tanto a compuestos como a simples.
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

        // Devuelve la lista de hijos directos (compuestos + simples).
        public List<IPermiso> ObtenerHijos() => Hijos;

        // Para depuración: retorna el nombre del permiso.
        public override string ToString() => Nombre;

        // Igualdad basada en el ID (único).
        public override bool Equals(object? obj) =>
            obj is PermisoCompuesto pc && pc.ID == ID;

        public override int GetHashCode() => ID.GetHashCode();
    }
}
