using ClienteApi.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ClienteApi.ExecuteTRA
{
    public class ExecuteTRA
    {
        private readonly ClienteApiConsumer _clienteApiConsumer;
        private readonly ILogger<ExecuteTRA> _logger;

        public ExecuteTRA(ClienteApiConsumer clienteApiConsumer, ILogger<ExecuteTRA> logger)
        {
            _clienteApiConsumer = clienteApiConsumer;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            try
            {
                var channel = _clienteApiConsumer.GetChannel();

                await channel.QueueDeclareAsync(
                    queue: RabbitMQConfig.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation($"Mensagem recebida: {message}");
                    _logger.LogInformation("Simulando envio de e-mail para notificacao@empresa.com com dados do cliente...");

                    await Task.CompletedTask;
                };

                await channel.BasicConsumeAsync(
                    queue: RabbitMQConfig.QueueName,
                    autoAck: true,
                    consumer: consumer
                );

                _logger.LogInformation("RabbitMQ Consumer rodando... (pressione Ctrl+C para sair)");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
