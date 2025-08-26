#nullable disable

namespace SocialMediaDataScraper.Models
{
    public static class ExceptionExtensions
    {
        public static List<string> GetAllInnerMessages(this Exception ex)
        {
            var messages = new List<string>();

            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return messages;
        }
    }
}
