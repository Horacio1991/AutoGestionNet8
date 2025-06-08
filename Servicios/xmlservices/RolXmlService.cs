using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices

{
    public static class RolXmlService
    {
        private static readonly string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "roles.xml");

        public static void Guardar(List<PermisoCompuesto> roles)
        {
            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
            serializer.Serialize(writer, roles);
        }

        public static List<PermisoCompuesto> Leer()
        {
            if (!File.Exists(ruta))
                return new List<PermisoCompuesto>();

            using var reader = new StreamReader(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
            return (List<PermisoCompuesto>)serializer.Deserialize(reader);
        }
    }
}
