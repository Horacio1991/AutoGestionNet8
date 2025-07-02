using System.Xml.Serialization;

namespace AutoGestion.DAO.Repositorios
{
    // Este repositorio generico lee y escribe en archivos XML.

    public class XmlRepository<T>
    {
        private readonly string _archivo;
        // Inicializa el repositorio creando la carpeta y archivo si no existen.
        public XmlRepository(string nombreArchivo)
        {
            // 1) Determinar y asegurar directorio
            var carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
            Directory.CreateDirectory(carpeta);

            _archivo = Path.Combine(carpeta, nombreArchivo);

            // 2) Crear archivo vacío si no existe
            if (!File.Exists(_archivo))
            {
                try
                {
                    GuardarLista(new List<T>());
                }
                catch (IOException ex)
                {
                    throw new ApplicationException($"No se pudo crear el archivo {_archivo}.", ex);
                }
            }
        }

        // Obtiene todos los elementos almacenados en el XML.
        public List<T> ObtenerTodos()
        {
            try
            {
                using var fs = new FileStream(_archivo, FileMode.Open, FileAccess.Read, FileShare.Read);
                var serializer = new XmlSerializer(typeof(List<T>));
                return (List<T>)serializer.Deserialize(fs)!;
            }
            catch (InvalidOperationException ex)
            {
                // error de deserialización
                throw new ApplicationException($"Error al leer {_archivo}: formato inválido.", ex);
            }
            catch (IOException ex)
            {
                // error de I/O
                throw new ApplicationException($"Error de acceso a {_archivo}.", ex);
            }
        }

        // Agrega un nuevo elemento al XML.
        // <param name="item">Instancia de T a agregar.</param>
        public void Agregar(T item)
        {
            // 1) Cargar lista actual
            var lista = ObtenerTodos();
            // 2) Agregar nuevo ítem
            lista.Add(item);
            // 3) Guardar cambios
            GuardarLista(lista);
        }

        // Serializa y sobreescribe el archivo XML con la lista completa.
        // <param name="lista">Lista de T a persistir.</param>
        public void GuardarLista(List<T> lista)
        {
            try
            {
                using var fs = new FileStream(_archivo, FileMode.Create, FileAccess.Write, FileShare.None);
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(fs, lista);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"Error al serializar datos en {_archivo}.", ex);
            }
            catch (IOException ex)
            {
                throw new ApplicationException($"Error de escritura en {_archivo}.", ex);
            }
        }
    }
}
