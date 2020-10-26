namespace Shared.MessageBus
{
    public class RabbitMQSettings
    {
        public readonly string Hostname;
        public readonly string Username;
        public readonly string Password;

        public RabbitMQSettings(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
        }
    }
}
