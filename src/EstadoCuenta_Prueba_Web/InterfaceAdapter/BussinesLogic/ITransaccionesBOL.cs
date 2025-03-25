using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITransaccionesBOL
    {
        public Task<List<TransaccionesViewModel>> GetTransacciones(int tarjetaId);
    }
}
