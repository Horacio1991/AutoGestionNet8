namespace AutoGestion.DTOs
{
    public class DashboardRankingDto
    {
        public string Vendedor { get; set; }

        /// Total facturado por ese vendedor en el período seleccionado.
        public decimal Total { get; set; }
    }
}
