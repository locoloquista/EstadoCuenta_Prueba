using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TarjetaCreditoViewModel
    {
        [Display(Name = "Tarjeta Id")]
        public long TarjetaId { get; set; }
        [Display(Name = "Numero de Tarjeta")]
        public string NumeroTarjeta { get; set; }
        [Display(Name = "Limite de Crédito")]
        public decimal LimiteCredito { get; set; }
        [Display(Name = "Saldo Actual")]
        public decimal SaldoActual { get; set; }
        [Display(Name = "Monto Disponible")]
        public decimal MontoDisponible { get; set; }


        // Propiedades formateadas
        public string LimiteCreditoFormateado => LimiteCredito.ToString("C2");
        public string SaldoActualFormateado => SaldoActual.ToString("C2");
        public string MontoDisponibleFormateado => MontoDisponible.ToString("C2");
    }
}
