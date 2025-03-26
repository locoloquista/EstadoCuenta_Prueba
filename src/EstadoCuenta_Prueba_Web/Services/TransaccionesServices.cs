using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using Services.Consumer.ConsumerFactory;
using Services.Consumer;
using InterfaceAdapter.DTO.Communications;

namespace Services
{
    public class TransaccionesServices : ITransaccionesServices
    {
        private readonly IRestApiConsumer _apiConsumer;
        private string controller = "Transacciones";

        public TransaccionesServices(IConsumerFactory consumerFactory)
        {
            _apiConsumer = consumerFactory.GetURLServices(Service.EstadoCuentaApi);
        }
        public async Task<List<TransaccionDTO>> GetTransacciones(int tarjetaId)
        {
            var action = "GetTransaccionesByIdTarjeta";
            var request = new { idTarjeta = tarjetaId };
            var result = await Task.Run(() => _apiConsumer.Consume<GenericListResponse<TransaccionDTO>>(controller, action, request));

            if (result.Items == null)
            {
                return new List<TransaccionDTO>();
            }

            return result.Items;
        }
    }
}
