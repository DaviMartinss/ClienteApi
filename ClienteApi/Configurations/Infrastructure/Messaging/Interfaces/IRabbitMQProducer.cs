namespace ClienteApi.Configurations.Infrastructure.Messaging.Interfaces
{
    public interface IRabbitMQProducer
    {
        void SendMessage<T>(T message);
    }
}