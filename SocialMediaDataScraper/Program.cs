using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;

namespace SocialMediaDataScraper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var list = DbHelper.GetAll<DS_Browser>();
            ApplicationConfiguration.Initialize();
            Application.Run(new FormDsBrowser(list.First()));
        }
    }
}