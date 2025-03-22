namespace Infrastructure.DataBase.Repository
{
    public class ClienteRepository
    {
        public long ClienteId { get; set; }
        public string NombreCompleto { get; set; }
        public long TarjetaId { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal MontoDisponible { get; set; }
    }
}
