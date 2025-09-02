namespace SocialMediaDataScraper.Models
{
    public static class StaticInfo
    {
        public static string DefaultUserAgent { get; set; } = "Mozilla/5.0 (iPhone; CPU iPhone OS 18_6_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1";
        public static string UserSessionDirectory
        {
            get
            {
                var userDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSessions");

                if (!System.IO.File.Exists(userDataFolder))
                {
                    Directory.CreateDirectory(userDataFolder);
                }

                return userDataFolder;
            }
        }

        public static string NormalizeUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string url = input.Trim();

            // Check protocol
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }

            // Insert www if missing (after protocol)
            Uri uri = new Uri(url);
            string host = uri.Host;

            if (!host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                host = "www." + host;
            }

            // Rebuild URL with correct host
            UriBuilder builder = new UriBuilder(uri)
            {
                Host = host
            };

            return builder.ToString();
        }
    }
}
