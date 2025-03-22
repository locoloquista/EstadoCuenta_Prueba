using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.DataAccess
{
    public interface ITarjetaCreditoDAO
    {
        Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int IdCliente);
    }
}
