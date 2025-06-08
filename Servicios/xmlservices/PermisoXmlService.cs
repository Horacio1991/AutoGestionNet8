using AutoGestion.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.XmlServices

{
    public static class PermisoXmlService
    {
        private static string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "permisos.xml");

        public static void Guardar(List<PermisoSimple> permisos)
        {
            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
            serializer.Serialize(writer, permisos);
        }

        public static List<PermisoSimple> Leer()
        {
            if (!File.Exists(ruta))
                return new List<PermisoSimple>();

            using var reader = new StreamReader(ruta);
            var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
            return (List<PermisoSimple>)serializer.Deserialize(reader);
        }
    }
}
