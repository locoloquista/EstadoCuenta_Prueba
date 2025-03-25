using InterfaceAdapter.ConsumerServices;
using InterfaceAdapter.DTO;
using InterfaceAdapter.DTO.Communications;
using Services.Consumer;
using Services.Consumer.ConsumerFactory;

namespace Services
{
    public class EstadoCuentaServices : IEstadoCuentaServices
    {
        private readonly IRestApiConsumer _apiConsumer;
        private string controller = "EstadoCuenta";

        public EstadoCuentaServices(IConsumerFactory consumerFactory)
        {
            _apiConsumer = consumerFactory.GetURLServices(Service.EstadoCuentaApi);
        }
        public async Task<EstadoCuentaDTO> DetalleEstadoCuenta(int tarjetaId)
        {
            var action = "GetEstadoCuentaByIdTarjeta";
            var request = new { IdTarjetaCredito = tarjetaId };
            var result = await Task.Run(() => _apiConsumer.Consume<GenericResponse<EstadoCuentaDTO>>(controller, action, request));

            if (result.Item == null)
            {
                throw new Exception("Error al obtener las transacciones");
            }

            return result.Item;
        }
    }
}
