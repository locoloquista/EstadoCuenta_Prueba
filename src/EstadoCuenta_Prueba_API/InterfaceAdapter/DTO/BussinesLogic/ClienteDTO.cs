namespace InterfaceAdapter.DTO.BussinesLogic
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; }
        public int TarjetaId { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal MontoDisponible { get; set; }
    }
}
