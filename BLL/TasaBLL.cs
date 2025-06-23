using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.BLL
{
    public class TasaBLL
    {
        // Repositorio donde se guardarán las tasaciones en formato XML
        private readonly XmlRepository<Tasacion> _repo = new("tasaciones.xml");

        public void RegistrarTasacion(OfertaCompra oferta, decimal valorFinal)
        {
            Tasacion tasacion = new()
            {
                ID = GeneradorID.ObtenerID<Tasacion>(),
                Oferta = oferta,
                ValorFinal = valorFinal,
                Fecha = DateTime.Now
            };

            _repo.Agregar(tasacion);
        }

        public RangoTasacion CalcularRangoTasacion(string modelo, string estadoMotor, int kilometraje)
        {
            decimal basePrice = 2500000;

            if (estadoMotor == "Excelente") basePrice += 300000;
            if (estadoMotor == "Regular") basePrice -= 200000;
            if (kilometraje > 100000) basePrice -= 200000;

            return new RangoTasacion
            {
                Min = basePrice * 0.9m,
                Max = basePrice * 1.1m
            };
        }

    }
}
