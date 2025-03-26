using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.DTO.Communications;
using Services.Consumer;
using Services.Consumer.ConsumerFactory;

namespace Services
{
    public class ClienteServices : IClienteServices
    {
        private readonly IRestApiConsumer _apiConsumer;
        private string controller = "Cliente";

        public ClienteServices(IConsumerFactory consumerFactory)
        {
            _apiConsumer = consumerFactory.GetURLServices(Service.EstadoCuentaApi);
        }

        public async Task<List<ClienteDTO>> GetAllCliente()
        {
            var action = "GetAllCliente";
            var result = await Task.Run(() => _apiConsumer.ConsumeWithJsonRequest<GenericListResponse<ClienteDTO>>(controller, action, RestSharp.Method.Get));
            
            if (result.Items == null)
            {
                return new List<ClienteDTO>();
            }
            return result.Items;
        }

        public async Task<ClienteDTO> GetClientebyId(int idCliente)
        {
            var action = "GetClientebyId";
            var request = new { idCliente };
            var result = await Task.Run(() => _apiConsumer.Consume<GenericResponse<ClienteDTO>>(controller, action, request));

            if (result.Item == null)
            {
                return new ClienteDTO();
            }

            return result.Item;
        }
    }
}
