using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class FacturaBLL
    {
        private readonly XmlRepository<Factura> _repo;


        /// 1) Inicializa el repositorio apuntando a "DatosXML/facturas.xml".
        public FacturaBLL()
        {
            _repo = new XmlRepository<Factura>("facturas.xml");
        }

        public Factura EmitirFactura(Factura factura)
        {
            try
            {
                // 1) Asignar nuevo ID único
                factura.ID = GeneradorID.ObtenerID<Factura>();

                // 2) Establecer fecha de emisión
                factura.Fecha = DateTime.Now;

                // 3) Cargar lista existente de facturas
                var lista = _repo.ObtenerTodos();

                // 4) Agregar la nueva factura
                lista.Add(factura);
                // 5) Guardar lista actualizada en el XML
                _repo.GuardarLista(lista);

                // 6) Devolver la factura con datos completos
                return factura;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException(
                    $"Error al emitir la factura: {ex.Message}", ex);
            }
        }
    }
}
