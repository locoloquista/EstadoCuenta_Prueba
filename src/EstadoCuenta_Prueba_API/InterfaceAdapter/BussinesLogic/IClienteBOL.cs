using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IClienteBOL
    {
        Task<List<ClienteDTO>> GetAllCliente();
        Task<ClienteDTO> GetClientebyId(int idCliente);
    }
}
