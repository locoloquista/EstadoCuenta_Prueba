using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TransaccionesViewModel
    {
        [Display(Name = "Tarjeta ID")]
        public int TarjetaId { get; set; }
        [Display(Name = "Tipo de Transacción")]
        public string TipoTransaccion { get; set; }
        [Display(Name = "Fecha de Transacción")]
        public DateTime Fecha { get; set; }
        [Display(Name = "Concepto")]
        public string Descripcion { get; set; }
        [Display(Name = "Monto de Transacción")]
        public decimal Monto { get; set; }
    }
}
