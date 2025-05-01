using ClienteApi.Consumer.ClienteApi.Consumer.Transaction;
using ClienteApi.Consumer.ClienteApi.Consumer.Util;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ClienteApi.SendClientRegisteredEmail.Transaction
{
    public class ExecuteTRA
    {
        private readonly SendClientRegisteredEmailTRA _clienteApiConsumer;
        private readonly ILogger<ExecuteTRA> _logger;

        public ExecuteTRA(SendClientRegisteredEmailTRA clienteApiConsumer, ILogger<ExecuteTRA> logger)
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
                    queue: SendClientRegisteredEmailConfig.QueueName,
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
                    queue: SendClientRegisteredEmailConfig.QueueName,
                    autoAck: true,
                    consumer: consumer
                );

                _logger.LogInformation("RabbitMQ Consumer rodando...");
                Console.ReadLine();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}