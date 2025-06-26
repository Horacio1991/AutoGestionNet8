namespace AutoGestion.DTOs
{
    /// <summary>
    /// DTO de entrada para registrar el estado final del vehículo de una oferta.
    /// </summary>
    public class RegistrarDatosInputDto
    {
        public int OfertaID { get; set; }
        public string EstadoStock { get; set; }
    }
}
