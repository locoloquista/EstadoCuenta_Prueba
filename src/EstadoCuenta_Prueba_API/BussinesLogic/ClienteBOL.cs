using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.DataAccess;
using InterfaceAdapter.DTO.BussinesLogic;

namespace BussinesLogic
{
    public class ClienteBOL : IClienteBOL
    {
        readonly IClienteDAO _clienteDAO;

        public ClienteBOL(IClienteDAO clienteDAO)
        {
            _clienteDAO = clienteDAO;
        }

        public async Task<List<ClienteDTO>> GetAllCliente()
        {
            return await _clienteDAO.GetAllCliente();
        }

        public async Task<ClienteDTO> GetClientebyId(int idCliente)
        {
            return await _clienteDAO.GetClientebyId(idCliente);
        }
    }
}
