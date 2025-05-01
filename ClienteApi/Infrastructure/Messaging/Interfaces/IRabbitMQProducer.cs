namespace ClienteApi.Infrastructure.Messaging.Interfaces
{
    public interface IRabbitMQProducer
    {
        void SendMessage<T>(T message);
    }
}
