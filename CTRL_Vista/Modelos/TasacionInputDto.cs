namespace AutoGestion.DTOs
{
    public class TasacionInputDto
    {
        // Oferta que se esta Tasando
        public int OfertaID { get; set; }
        public decimal ValorFinal { get; set; }
        public string EstadoStock { get; set; } // Disponible, requiere reacondicionamiento
    }
}
