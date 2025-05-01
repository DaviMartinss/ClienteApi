using ClienteApi.Application.Interfaces;
using ClienteApi.Configurations.Infrastructure.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ClienteApi.Infrastructure.Messaging
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            // Criando a conexão com o RabbitMQ
            _connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public void SendMessage<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json));

            var basicProperties = new BasicProperties
            {
                ContentType = "application/json"
            };

            SendMessageAsync(RabbitMQConfiguracao.ExchangeName, RabbitMQConfiguracao.RoutingKey, false, basicProperties, body, CancellationToken.None)
                .GetAwaiter().GetResult();
        }

        public async ValueTask SendMessageAsync<TProperties>(string exchange, string routingKey,
    bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body,
    CancellationToken cancellationToken = default)
        where TProperties : IReadOnlyBasicProperties, IAmqpHeader
        {
            try
            {
                await _channel.ExchangeDeclareAsync(
                    exchange: RabbitMQConfiguracao.ExchangeName,
                    type: ExchangeType.Direct,
                    durable: false,
                    autoDelete: false
                );

                await _channel.QueueDeclareAsync(
                    queue: RabbitMQConfiguracao.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                await _channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    mandatory: mandatory,
                    basicProperties: basicProperties,
                    body: body,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
