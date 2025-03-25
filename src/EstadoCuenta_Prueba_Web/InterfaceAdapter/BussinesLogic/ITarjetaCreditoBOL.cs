using ViewModels;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITarjetaCreditoBOL
    {
        Task<List<TarjetaCreditoViewModel>> GetTartejaCreditoByClienteId(int IdCliente);
    }
}
