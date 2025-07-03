using System.Xml.Serialization;
using AutoGestion.DAO.Modelos;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.Servicios.XmlServices
{
    /// Servicio para leer y guardar usuarios (y sus roles) en usuarios.xml.
    /// Asegura que siempre exista un usuario "admin" con permisos completos.
    public static class UsuarioXmlService
    {
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DatosXML",
            "usuarios.xml");

        // Lee los usuarios desde el XML (tipo UsuarioSerializable), los mapea a Usuario,
        // asigna roles cargados desde roles.xml y permisos simples desde permisos.xml,
        // y garantiza que exista un usuario "admin".
        public static List<Usuario> Leer()
        {
            try
            {
                List<UsuarioSerializable> serializables;
                if (!File.Exists(_ruta))
                {
                    serializables = new List<UsuarioSerializable>();
                }
                else
                {
                    using var fs = new FileStream(_ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var ser = new XmlSerializer(typeof(List<UsuarioSerializable>));
                    serializables = (List<UsuarioSerializable>)ser.Deserialize(fs)!;
                }

                // Cargar roles y permisos
                var roles = RolXmlService.Leer();
                var simples = PermisoXmlService.Leer();

                // Mapear a Usuario (dominio)
                var usuarios = serializables.Select(s =>
                {
                    var u = new Usuario
                    {
                        ID = s.ID,
                        Nombre = s.Nombre,
                        Clave = s.Clave // ya viene encriptada
                    };
                    if (!string.IsNullOrWhiteSpace(s.RolNombre))
                    {
                        var rol = roles.FirstOrDefault(r => r.Nombre == s.RolNombre);
                        u.Rol = rol;
                    }
                    return u;
                }).ToList();

                return AsegurarAdmin(usuarios, roles, simples);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"El archivo de usuarios está corrupto: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al leer usuarios: {ex.Message}", ex);
            }
        }

        // Guarda la lista de usuarios en el XML (serializando a UsuarioSerializable).
        public static void Guardar(List<Usuario> usuarios)
        {
            try
            {
                var dir = Path.GetDirectoryName(_ruta)!;
                Directory.CreateDirectory(dir);

                var serializables = usuarios.Select(u => new UsuarioSerializable
                {
                    ID = u.ID,
                    Nombre = u.Nombre,
                    Clave = u.Clave,
                    RolNombre = (u.Rol as PermisoCompuesto)?.Nombre
                }).ToList();

                using var writer = new StreamWriter(_ruta);
                var ser = new XmlSerializer(typeof(List<UsuarioSerializable>));
                ser.Serialize(writer, serializables);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar usuarios: {ex.Message}", ex);
            }
        }

        // Elimina del XML al usuario con el nombre dado.
        public static void Eliminar(string nombreUsuario)
        {
            try
            {
                var lista = Leer();
                lista.RemoveAll(u => u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));
                Guardar(lista);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar usuario: {ex.Message}", ex);
            }
        }

        // Asegura que exista el usuario "admin" con rol SuperAdmin (todos los permisos).
        // Si no existe, lo crea y actualiza el XML.
        private static List<Usuario> AsegurarAdmin(
            List<Usuario> lista,
            List<PermisoCompuesto> roles,
            List<PermisoSimple> permisosSimples)
        {
            const string adminName = "admin";
            if (lista.Any(u => u.Nombre.Equals(adminName, StringComparison.OrdinalIgnoreCase)))
                return lista;

            // Crear rol SuperAdmin con todos los permisos simples
            var super = new PermisoCompuesto
            {
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = "SuperAdmin",
                HijosSimples = permisosSimples.ToList()
            };

            // Construir usuario admin
            var admin = new Usuario
            {
                ID = GeneradorID.ObtenerID<Usuario>(),
                Nombre = adminName,
                Clave = Encriptacion.Encriptacion.EncriptarPassword("123"),
                Rol = super
            };

            lista.Add(admin);
            Guardar(lista);
            return lista;
        }
    }
}
