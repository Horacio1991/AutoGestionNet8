using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;// Define PermisoCompuesto

namespace AutoGestion.Servicios.XmlServices

{
    public static class RolXmlService
    {
        // Ruta del archivo XML donde se guardarán los roles
        private static readonly string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "roles.xml");

        public static void Guardar(List<PermisoCompuesto> roles)
        {
            // Crea el archivo rolex.xml, crea un serializador XML y serializa la lista de roles en el archivo
            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
            serializer.Serialize(writer, roles);
        }

        public static List<PermisoCompuesto> Leer()
        {
            // Si no existe el archivo, devuelve una lista vacía
            if (!File.Exists(ruta))
                return new List<PermisoCompuesto>();

            // Abre el archivo en modo lectura, crea un serializador XML y deserializa el contenido del archivo en una lista de roles
            using var reader = new StreamReader(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
            return (List<PermisoCompuesto>)serializer.Deserialize(reader);
        }
    }
}
