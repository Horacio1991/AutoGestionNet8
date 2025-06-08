using AutoGestion.Entidades;

namespace AutoGestion.Vista.Modelos
{
    public class OfertaComboItem
    {
        public OfertaCompra Oferta { get; set; }

        public string Descripcion =>
            $"{Oferta.Vehiculo.Marca} {Oferta.Vehiculo.Modelo} - {Oferta.Vehiculo.Dominio}";

        public override string ToString() => Descripcion;
    }
}
