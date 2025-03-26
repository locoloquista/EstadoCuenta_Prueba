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
                return new TResponse();
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

        public TResponse ConsumeWithJsonRequestFromClass<TResponse, TRequest>(string controller, string action, TRequest request, Method method = Method.Post) where TResponse : class, new() where TRequest : class
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
