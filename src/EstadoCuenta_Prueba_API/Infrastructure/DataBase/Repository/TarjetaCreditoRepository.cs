namespace Infrastructure.DataBase.Repository
{
    public class TarjetaCreditoRepository
    {
        public long TarjetaId { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal MontoDisponible { get; set; }
    }
}
