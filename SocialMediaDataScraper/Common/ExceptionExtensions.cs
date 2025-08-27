#nullable disable

using System.Dynamic;

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

    public static class ObjectExtensions
    {
        public static ExpandoObject ToExpando(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var expando = new ExpandoObject();
            var dict = (IDictionary<string, object?>)expando;

            foreach (var property in obj.GetType().GetProperties())
            {
                dict[property.Name] = property.GetValue(obj);
            }

            return expando;
        }
    }
}
