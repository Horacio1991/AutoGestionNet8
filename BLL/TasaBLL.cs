using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;
using System;
using System.Collections.Generic;

namespace AutoGestion.BLL
{
    public class TasaBLL
    {
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

        public List<Tasacion> ObtenerTodas()
        {
            return _repo.ObtenerTodos();
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
