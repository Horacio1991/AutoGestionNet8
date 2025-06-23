using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite

{
    [Serializable]
    // Representa un permiso de menú que contiene un nombre de menú y una lista de elementos asociados.
    public class MenuPermiso
    {
        // Nombre del menu, por ejemplo, "Ventas", "Compras", etc.
        // Se corresponde con el nombre del menú en la interfaz de usuario.
        public string Menu { get; set; }

        // Lista de nombres de elementos (sub-opciones) dentro del menú.
        // Estos nombres se comparan con el texto de los toolStripMenuItem del DropDownMenu.
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<string> Items { get; set; } = new();
    }
}
