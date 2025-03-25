using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface ITarjetaCreditoServices
    {
        Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int idCliente);
    }
}
