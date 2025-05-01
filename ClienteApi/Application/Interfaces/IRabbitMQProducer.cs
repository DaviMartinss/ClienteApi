namespace ClienteApi.Application.Interfaces
{
    public interface IRabbitMQProducer
    {
        void SendMessage<T>(T message);
    }
}
