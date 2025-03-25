using RestSharp;

namespace Services.Consumer
{
    public interface IRestApiConsumer
    {
        TResponse ConsumeWithJsonRequest<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new();
        TResponse Consume<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new();
    }
}
