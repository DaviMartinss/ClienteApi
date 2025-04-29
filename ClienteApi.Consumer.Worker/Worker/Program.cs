using ClienteApi.Consumer.ClienteApi.Consumer.Transaction;
using ClienteApi.SendClientRegisteredEmail.Transaction;

namespace ClienteApi.SendClientRegisteredEmail.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Configura��o do RabbitMQ
                    services.AddSingleton<SendClientRegisteredEmailTRA>();

                    // Configura��o do Worker
                    services.AddSingleton<ExecuteTRA>();
                    services.AddHostedService<Worker>();

                    // Configura��o de logs
                    services.AddLogging(builder => builder.AddConsole());
                });
    }
}
