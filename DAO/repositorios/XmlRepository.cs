using System.Xml.Serialization;

namespace AutoGestion.DAO.Repositorios
{
    // Clase usada para persistir objetos en XML (crear, leer, actualizar y eliminar)
    public class XmlRepository<T>
    {
        // Ruta del archivo XML donde se guardarán los datos
        private readonly string _archivo;

        // Constructor que recibe el nombre del archivo XML y crea el directorio si no existe
        public XmlRepository(string nombreArchivo)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
            Directory.CreateDirectory(folder);
            _archivo = Path.Combine(folder, nombreArchivo);

            if (!File.Exists(_archivo))
                GuardarLista(new List<T>());
        }

        // Leer todos los elementos del archivo XML y los devuelve como una lista
        public List<T> ObtenerTodos()
        {
            using var fs = new FileStream(_archivo, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<T>)); //Crea un serializador para una lista de tipo T
            return (List<T>)serializer.Deserialize(fs); //Deserializa el contenido y lo devuelve
        }

        // Agrega un elemento en el archivo XML (obtiene todos los elementos, agrega el nuevo y guarda la lista)
        public void Agregar(T item)
        {
            var lista = ObtenerTodos();
            lista.Add(item);
            GuardarLista(lista);
        }

        // Serializa y sobreescribe el archivo XML con la lista proporcionada
        public void GuardarLista(List<T> lista)
        {
            using var fs = new FileStream(_archivo, FileMode.Create);
            var serializer = new XmlSerializer(typeof(List<T>));
            serializer.Serialize(fs, lista);
        }

    }
}
