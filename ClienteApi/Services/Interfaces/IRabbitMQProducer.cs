namespace ClienteApi.Services.Interfaces
{
    public interface IRabbitMQProducer
    {
        void SendMessage<T>(T message);
    }
}
