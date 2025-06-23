using AutoGestion.Servicios.Composite; // Contiene la definición de PermisoCompleto
using System.Xml.Serialization;

namespace AutoGestion.Servicios.XmlServices

{
    public static class PermisoCompletoXmlService
    {
        // Ruta al archivo XML donde se almacenan los permisos completos
        private static string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "permisos.xml");

        // Deserializa una lista de PermisoCompleto desde el archivo XML
        // Si no existe el archivo, retorna una lista vacía
        public static List<PermisoCompleto> Leer()
        {
            // Si no existe el XML, no hay permisos para cargar
            if (!File.Exists(ruta)) return new List<PermisoCompleto>();

            // Abrimos el archivo XML y deserializamos su contenido
            using var stream = new FileStream(ruta, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<PermisoCompleto>));

            // convertimos el xml y retornamos la lista de permisos completos
            return (List<PermisoCompleto>)serializer.Deserialize(stream);
        }

        public static void Guardar(List<PermisoCompleto> permisos)
        {
            // Abrimos el archivo XML para escribir los permisos completos
            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompleto>));
            // Serializamos la lista de permisos completos al archivo XML
            serializer.Serialize(writer, permisos);
        }
    }
}
