using InterfaceAdapter.DTO;

namespace InterfaceAdapter.ConsumerServices
{
    public interface IClienteServices
    {
        Task<List<ClienteDTO>> GetAllCliente();
        Task<ClienteDTO> GetClientebyId(int idCliente);
    }
}
