using RabbitMQ.Client;

namespace ClienteApi.Consumer.ClienteApi.Consumer.Transaction
{
    public class SendClientRegisteredEmailTRA
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public SendClientRegisteredEmailTRA(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
                Password = _configuration.GetValue<string>("RabbitMQ:Password")
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public IChannel GetChannel() => _channel;
    }
}
