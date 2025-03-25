namespace InterfaceAdapter.DTO
{
    public class EstadoCuentaDTO
    {
        public string NumeroTarjeta { get; set; }
        public string NombreCliente { get; set; }
        public decimal SaldoTotal { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoDisponible { get; set; }
        public decimal ComprasMesActual { get; set; }
        public decimal ComprasMesAnterior { get; set; }
        public decimal TasaInteres { get; set; }
        public decimal InteresBonificable { get; set; }
        public decimal CuotaMinima { get; set; }
        public decimal PagoContadoConIntereses { get; set; }
    }
}
