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
            try
            {
                _configuration = configuration;

                var factory = new ConnectionFactory
                {
                    HostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
                    UserName = _configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
                    Password = _configuration.GetValue<string>("RabbitMQ:Password") ?? "guest"
                };

                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public IChannel GetChannel() => _channel;
    }
}
