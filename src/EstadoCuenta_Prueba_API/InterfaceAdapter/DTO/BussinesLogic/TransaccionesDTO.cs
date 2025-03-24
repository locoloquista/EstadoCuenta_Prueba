namespace InterfaceAdapter.DTO.BussinesLogic
{
    public class TransaccionesDTO
    {
        public string TipoTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}
