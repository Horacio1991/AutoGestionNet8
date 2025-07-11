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

        public bool RegistrarPago(Pago pago)
        {
            try
            {
                pago.ID = GeneradorID.ObtenerID<Pago>();
                var lista = _repo.ObtenerTodos();
                lista.Add(pago);
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
