namespace ClienteApi.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ClienteApi.ExecuteTRA.ExecuteTRA _executeTRA;
        private readonly ILogger<Worker> _logger;

        public Worker(ClienteApi.ExecuteTRA.ExecuteTRA executeTRA, ILogger<Worker> logger)
        {
            _executeTRA = executeTRA;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando o Worker...");
            await _executeTRA.StartAsync();
        }
    }
}
