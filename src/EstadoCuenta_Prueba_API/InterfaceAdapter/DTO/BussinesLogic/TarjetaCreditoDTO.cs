namespace InterfaceAdapter.DTO.BussinesLogic
{
    public class TarjetaCreditoDTO
    {
        public long TarjetaId { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal MontoDisponible { get; set; }
    }
}
