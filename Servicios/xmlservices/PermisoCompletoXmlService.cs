using AutoGestion.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.XmlServices

{
    public static class PermisoCompletoXmlService
    {
        private static string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "permisos.xml");

        public static List<PermisoCompleto> Leer()
        {
            if (!File.Exists(ruta)) return new List<PermisoCompleto>();

            using var stream = new FileStream(ruta, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<PermisoCompleto>));
            return (List<PermisoCompleto>)serializer.Deserialize(stream);
        }

        public static void Guardar(List<PermisoCompleto> permisos)
        {
            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoCompleto>));
            serializer.Serialize(writer, permisos);
        }
    }
}
