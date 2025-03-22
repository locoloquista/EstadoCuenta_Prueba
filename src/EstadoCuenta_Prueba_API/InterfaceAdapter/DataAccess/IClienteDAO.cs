using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.DataAccess
{
    public interface IClienteDAO
    {
        Task<List<ClienteDTO>> GetAllCliente();
        Task<ClienteDTO> GetClientebyId(int idCliente);
    }
}
