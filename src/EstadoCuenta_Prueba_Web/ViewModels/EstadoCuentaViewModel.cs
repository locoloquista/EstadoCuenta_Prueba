using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class EstadoCuentaViewModel
    {
        [Display(Name = "Numero de Tarjeta")]
        public string NumeroTarjeta { get; set; }
        [Display(Name = "Nombre del Cliente")]
        public string NombreCliente { get; set; }
        [Display(Name = "Saldo Total")]
        public decimal SaldoTotal { get; set; }
        [Display(Name = "Limite de Credito")]
        public decimal LimiteCredito { get; set; }
        [Display(Name = "Saldo Dsiponible")]
        public decimal SaldoDisponible { get; set; }
        [Display(Name = "Compras del mes Actual")]
        public decimal ComprasMesActual { get; set; }
        [Display(Name = "Compras del mes anterior")]
        public decimal ComprasMesAnterior { get; set; }
        [Display(Name = "Tasa de Interes")]
        public decimal TasaInteres { get; set; }
        [Display(Name = "Interes Bonificable")]
        public decimal InteresBonificable { get; set; }
        [Display(Name = "Cuota Minima")]
        public decimal CuotaMinima { get; set; }
        [Display(Name = "Pago de Ccontado (con intereses)")]
        public decimal PagoContadoConIntereses { get; set; }
    }
}
