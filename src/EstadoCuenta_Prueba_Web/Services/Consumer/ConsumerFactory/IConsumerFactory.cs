namespace Services.Consumer.ConsumerFactory
{
    public interface IConsumerFactory
    {
        IRestApiConsumer GetURLServices(Service typeURLServices);
    }
}
