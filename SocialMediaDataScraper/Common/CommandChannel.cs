#nullable disable

namespace SocialMediaDataScraper.Common
{
    public class CommandChannel
    {
        public static event Action<string, object> OnReceiveCommand;

        public static void SendCommand<T>(string command, T data) where T : class, new()
        {
            OnReceiveCommand?.Invoke(command, data);
        }          
    }
}