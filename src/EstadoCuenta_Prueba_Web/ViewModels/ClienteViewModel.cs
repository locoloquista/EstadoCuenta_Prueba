using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class ClienteViewModel
    {
        [Display(Name ="Cliente")]
        public long ClienteId { get; set; }
        [Display(Name = "Nombre")]
        public string NombreCompleto { get; set; }
        [Display(Name = "Numero de Tarjetas")]
        public int NumeroTarjetasActivas { get; set; }
    }
}
