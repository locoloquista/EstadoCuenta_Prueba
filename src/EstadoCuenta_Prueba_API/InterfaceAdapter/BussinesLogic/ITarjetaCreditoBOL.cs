using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.BussinesLogic
{
    public interface ITarjetaCreditoBOL
    {
        Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int IdCliente);
    }
}
