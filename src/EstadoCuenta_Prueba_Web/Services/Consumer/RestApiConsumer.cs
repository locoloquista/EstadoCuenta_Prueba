using RestSharp;

namespace Services.Consumer
{
    public class RestApiConsumer : IRestApiConsumer
    {
        private readonly IRestClient _restClient;

        public RestApiConsumer(string url)
        {
            _restClient = new RestClient(url);
        }

        public TResponse Consume<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new()
        {
            var restRequest = new RestRequest($"/api/{controller}/{action}", method);
            restRequest.AddObject(request);
            restRequest.Timeout = TimeSpan.FromMilliseconds(1200000);
            var restResponse = _restClient.Execute<TResponse>(restRequest);

            if (restResponse.ErrorException != null)
            {
                throw new Exception($"Error: {restResponse.ErrorMessage}");
            }

            return restResponse.Data;
        }

        public TResponse ConsumeWithJsonRequest<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new()
        {
            var restRequest = new RestRequest($"/api/{controller}/{action}", method);
            restRequest.AddJsonBody(request);
            restRequest.Timeout = TimeSpan.FromMilliseconds(1200000);
            var restResponse = _restClient.Execute<TResponse>(restRequest);

            if (restResponse.ErrorException != null)
            {
                return new TResponse();
            }

            return restResponse.Data;
        }
    }
}
