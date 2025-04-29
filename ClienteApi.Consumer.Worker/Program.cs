using ClienteApi.ExecuteTRA;
using ClienteApi.Consumer;

namespace ClienteApi
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
                    // Configuração do RabbitMQ
                    services.AddSingleton<ClienteApiConsumer>();

                    // Configuração do Worker
                    services.AddSingleton<ClienteApi.ExecuteTRA.ExecuteTRA>();
                    services.AddHostedService<ClienteApi.Worker.Worker>();

                    // Configuração de logs
                    services.AddLogging(builder => builder.AddConsole());
                });
    }
}
