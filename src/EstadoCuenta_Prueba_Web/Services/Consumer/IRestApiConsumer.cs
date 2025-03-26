using RestSharp;

namespace Services.Consumer
{
    public interface IRestApiConsumer
    {
        TResponse ConsumeWithJsonRequest<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new();
        TResponse Consume<TResponse>(string controller, string action, object request, Method method = Method.Get) where TResponse : new();
        TResponse ConsumeWithJsonRequestFromClass<TResponse, TRequest>(string controller, string action, TRequest request, Method method = Method.Get) where TResponse : class, new () where TRequest : class;
    }
}
