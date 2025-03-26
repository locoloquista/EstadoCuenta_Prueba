using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.DTO.Communications;
using Services.Consumer.ConsumerFactory;
using Services.Consumer;
using System.Net;

namespace Services
{
    public class TarjetaCreditoServices : ITarjetaCreditoServices
    {
        private readonly IRestApiConsumer _apiConsumer;
        private string controller = "TarjetaCredito";

        public TarjetaCreditoServices(IConsumerFactory consumerFactory)
        {
            _apiConsumer = consumerFactory.GetURLServices(Service.EstadoCuentaApi);
        }

        public async Task<List<TarjetaCreditoDTO>> GetTartejaCreditoByClienteId(int idCliente)
        {
            var action = "GetTartejaCreditoByClienteId";
            var request = new { IdCliente = idCliente };

            var result = await Task.Run(() => _apiConsumer.Consume<GenericListResponse<TarjetaCreditoDTO>>(controller, action, request));
            
            if (result.Items == null)
            {
                return new List<TarjetaCreditoDTO>();
            }

            return result.Items;
        }
    }
}
