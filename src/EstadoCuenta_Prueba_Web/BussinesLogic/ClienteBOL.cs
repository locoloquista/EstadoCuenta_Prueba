using InterfaceAdapter.BussinesLogic;
using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.Mapping;
using ViewModels;

namespace BussinesLogic
{
    public class ClienteBOL : IClienteBOL
    {
        private readonly IClienteServices _clienteServices;
        private readonly ITarjetaCreditoBOL _tarjetaCreditoBOL;
        private readonly IParser _parser;

        public ClienteBOL(IClienteServices clienteServices, ITarjetaCreditoBOL tarjetaCreditoBOL, IParser parser)
        {
            _clienteServices = clienteServices;
            _tarjetaCreditoBOL = tarjetaCreditoBOL;
            _parser = parser;
        }

        public async Task<List<ClienteViewModel>> GetAllCliente()
        {
            List<ClienteDTO> listadoClientes = await _clienteServices.GetAllCliente();

            List<ClienteViewModel> listadoClientesViewModel = _parser.Parse<List<ClienteViewModel>, List<ClienteDTO>>(listadoClientes);

            return listadoClientesViewModel;
        }

        public async Task<InformacionClienteViewModel> InformacionCliente(int idCliente)
        {
            ClienteDTO infoCliente = await _clienteServices.GetClientebyId(idCliente);

            List<TarjetaCreditoViewModel> listadoTarjetas = await _tarjetaCreditoBOL.GetTartejaCreditoByClienteId(idCliente);

            InformacionClienteViewModel resultViewModel = new InformacionClienteViewModel
            {
                Cliente = _parser.Parse<ClienteViewModel, ClienteDTO>(infoCliente),
                TarjetasCredito = listadoTarjetas
            };

            return resultViewModel;
        }
    }
}
