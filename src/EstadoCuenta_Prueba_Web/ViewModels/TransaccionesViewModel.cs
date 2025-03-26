using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TransaccionesViewModel
    {
        [Display(Name = "Tarjeta ID")]
        public int TarjetaId { get; set; }

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        [Display(Name = "Tipo de Transacción")]
        public string TipoTransaccion { get; set; }

        [Required(ErrorMessage = "La fecha de transacción es obligatoria.")]
        [Display(Name = "Fecha de Transacción")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio.")]
        [Display(Name = "Concepto")]
        [StringLength(50, ErrorMessage = "El concepto no debe exceder los 50 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El monto de la transacción es obligatorio.")]
        [Display(Name = "Monto de Transacción")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }


        // Propiedades formateadas
        public string FechaFormateada => Fecha.ToString("d");
        public string MontoFormateado => Monto.ToString("C2");

    }
}
