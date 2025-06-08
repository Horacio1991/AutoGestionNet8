using AutoGestion.DAO.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Utilidades

{
    public static class GeneradorID
    {
        private static readonly Dictionary<string, string> NombresArchivos = new()
        {
            { "Cliente", "clientes.xml" },
            { "Vehiculo", "vehiculos.xml" },
            { "Venta", "ventas.xml" },
            { "Pago", "pagos.xml" },
            { "Factura", "facturas.xml" },
            { "EvaluacionTecnica", "evaluaciones.xml" },
            { "OfertaCompra", "ofertas.xml" },
            { "Tasacion", "tasaciones.xml" },
            { "Turno", "turnos.xml" },
            { "Comision", "comisiones.xml" },
            { "ComprobanteEntrega", "comprobantes.xml" },
            { "Oferente", "oferentes.xml" },
            { "PermisoCompleto", "permisos.xml" },
            { "Usuario", "usuarios.xml" },
            { "PermisoCompuesto" , "roles.xml" }

        };

        public static int ObtenerID<T>()
        {
            string tipo = typeof(T).Name;

            string archivo = NombresArchivos.ContainsKey(tipo)
                ? NombresArchivos[tipo]
                : tipo.ToLower() + "s.xml";

            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", archivo);

            if (!File.Exists(ruta))
                return 1;

            // Si es Usuario, usamos UsuarioSerializable
            if (typeof(T).Name == "Usuario")
            {
                var serializer = new XmlSerializer(typeof(List<UsuarioSerializable>));
                using var stream = new FileStream(ruta, FileMode.Open);
                var lista = (List<UsuarioSerializable>)serializer.Deserialize(stream);
                return lista.Any() ? lista.Max(u => u.ID) + 1 : 1;
            }

            var serializerT = new XmlSerializer(typeof(List<T>));
            using var streamT = new FileStream(ruta, FileMode.Open);
            var listaT = (List<T>)serializerT.Deserialize(streamT);

            var prop = typeof(T).GetProperty("ID");
            if (prop == null) return 1;

            var ids = listaT
                .Select(item => (int?)prop.GetValue(item))
                .Where(id => id.HasValue)
                .Select(id => id.Value);

            return ids.Any() ? ids.Max() + 1 : 1;
        }


    }
}
