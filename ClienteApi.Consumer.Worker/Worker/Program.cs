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
                    // Registro de SendClientRegisteredEmailTRA com inje��o segura de IConfiguration
                    services.AddSingleton<SendClientRegisteredEmailTRA>(sp =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();
                        return new SendClientRegisteredEmailTRA(configuration);
                    });

                    // Registro do Worker e sua l�gica
                    services.AddSingleton<ExecuteTRA>();
                    services.AddHostedService<Worker>();

                    // Configura��o de logs
                    services.AddLogging(builder => builder.AddConsole());
                });
    }
}
