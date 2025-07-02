using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class PagoBLL
    {
        private readonly XmlRepository<Pago> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/pagos.xml".
        public PagoBLL()
        {
            _repo = new XmlRepository<Pago>("pagos.xml");
        }

        // Registra un pago y lo persiste en XML.
        // pago = Pago a registrar con datos de monto, fecha y vehículo.
        public bool RegistrarPago(Pago pago)
        {
            try
            {
                // 1) Asignar nuevo ID único
                pago.ID = GeneradorID.ObtenerID<Pago>();

                // 2) Obtener lista actual de pagos
                var lista = _repo.ObtenerTodos();

                // 3) Agregar el nuevo pago
                lista.Add(pago);

                // 4) Persistir la lista actualizada en XML
                _repo.GuardarLista(lista);

                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar pago: {ex.Message}", ex);
            }
        }
    }
}
