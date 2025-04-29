namespace ClienteApi.Consumer.ClienteApi.Consumer.Util
{
    public class SendClientRegisteredEmailConfig
    {
        public static string ExchangeName { get; } = "cliente_exchange";
        public static string QueueName { get; } = "cliente_cadastrado";
        public static string RoutingKey { get; } = "cliente_routing";
    }
}