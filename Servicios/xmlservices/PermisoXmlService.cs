using AutoGestion.Servicios.Composite; // Define PermisoSimple
using System.Xml.Serialization;

namespace AutoGestion.Servicios.XmlServices

{
    public static class PermisoXmlService
    {
        // Ruta del archivo XML donde se guardarán los permisos simples
        private static string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "permisos.xml");

        public static void Guardar(List<PermisoSimple> permisos)
        {
            // Abre o crea el archivo en modo escritura
            using var writer = new StreamWriter(ruta);
            // Creamos un serializador para List<PermisoSimple>
            var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
            // Serializamos la lista completa en el archivo
            serializer.Serialize(writer, permisos);
        }

        public static List<PermisoSimple> Leer()
        {
            // Si no existe el XML, no hay permisos que leer
            if (!File.Exists(ruta))
                return new List<PermisoSimple>();

            // Abre el archivo en modo lectura y deserializa la lista de permisos simples
            using var reader = new StreamReader(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
            return (List<PermisoSimple>)serializer.Deserialize(reader);
        }
    }
}
