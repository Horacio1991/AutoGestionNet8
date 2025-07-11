using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    // Para gestionar las tasaciones de vehículos de ofertas.

    public class TasaBLL
    {
        private readonly XmlRepository<Tasacion> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/tasaciones.xml".
        public TasaBLL()
        {
            _repo = new XmlRepository<Tasacion>("tasaciones.xml");
        }

        // Registra una tasación para una oferta con valor final especificado.
        public void RegistrarTasacion(OfertaCompra oferta, decimal valorFinal)
        {
            try
            {
                // 1) Crear objeto Tasacion
                var tasacion = new Tasacion
                {
                    ID = GeneradorID.ObtenerID<Tasacion>(),
                    Oferta = oferta,
                    ValorFinal = valorFinal,
                    Fecha = DateTime.Now
                };

                // 2) Persistir la nueva tasación en XML
                _repo.Agregar(tasacion);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar tasación: {ex.Message}", ex);
            }
        }

        // Calcula un rango sugerido de tasación según modelo, estado del motor y kilometraje.
        public RangoTasacion CalcularRangoTasacion(string modelo, string estadoMotor, int kilometraje)
        {
            // 1) Precio base estándar
            decimal basePrice = 4_500_000m;

            // 2) Ajustes según estado del motor
            if (estadoMotor.Equals("Excelente", StringComparison.OrdinalIgnoreCase))
                basePrice += 300_000m;
            else if (estadoMotor.Equals("Regular", StringComparison.OrdinalIgnoreCase))
                basePrice -= 200_000m;

            // 3) Ajuste por alto kilometraje
            if (kilometraje > 100_000)
                basePrice -= 200_000m;

            // 4) Calcular rango +/-10%
            return new RangoTasacion
            {
                Min = basePrice * 0.9m,
                Max = basePrice * 1.1m
            };
        }
    }
}
