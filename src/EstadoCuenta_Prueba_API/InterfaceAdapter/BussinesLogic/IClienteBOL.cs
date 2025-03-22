using InterfaceAdapter.DTO.BussinesLogic;

namespace InterfaceAdapter.BussinesLogic
{
    public interface IClienteBOL
    {
        Task<List<ClienteDTO>> GetAllCliente();
    }
}
