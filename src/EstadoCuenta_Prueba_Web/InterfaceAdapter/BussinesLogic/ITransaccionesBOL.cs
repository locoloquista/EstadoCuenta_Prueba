using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITransaccionesBOL
    {
        Task<List<TransaccionesViewModel>> GetTransacciones(int tarjetaId);
        Task<List<TiposTransaccionesViewModel>> GetTiposTransacciones();
        Task<List<TransaccionesViewModel>> AgregarCompraPagoByTarjeta(TransaccionesViewModel model);
    }
}
