using AutoGestion.Entidades;

namespace AutoGestion.Vista.Modelos
{
    //Para mostrar en un combobox con una descripción amigable
    public class OfertaComboItem
    {
        public OfertaCompra Oferta { get; set; }

        public string Descripcion =>
            $"{Oferta.Vehiculo.Marca} {Oferta.Vehiculo.Modelo} - {Oferta.Vehiculo.Dominio}";

        public override string ToString() => Descripcion;
    }
}
