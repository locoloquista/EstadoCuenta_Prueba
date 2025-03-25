using Microsoft.Extensions.Options;

namespace Services.Consumer.ConsumerFactory
{
    public enum  Service
    {
        EstadoCuentaApi
    }

    public class ConsumerFactory : IConsumerFactory
    {
        private readonly ApiSettings _apiSettings;

        public ConsumerFactory(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public IRestApiConsumer GetURLServices(Service typeURLServices)
        {
            switch (typeURLServices)
            {
                case Service.EstadoCuentaApi:
                    return new RestApiConsumer(_apiSettings.EstadoCuentaApi_url);
                default:
                    return null;
            }
        }
    }
}
