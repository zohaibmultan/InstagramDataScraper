#nullable disable

using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SocialMediaDataScraper.Models
{
    public static class WebViewHelper
    {
        public static string ScriptDirectory
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javascripts");

                if (!System.IO.File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public static async Task<Dictionary<string, string>> GetBrowserCookies(WebView2 webView, string domain)
        {
            if (webView.CoreWebView2 == null)
            {
                return new Dictionary<string, string>();
            }

            try
            {
                var cookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync(domain);
                var dirCookies = new Dictionary<string, string>();
                foreach (var cookie in cookies)
                {
                    dirCookies.Add(cookie.Name, cookie.Value);
                }

                return dirCookies;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        public static async Task<Dictionary<string, string>> GetBrowserLocalStorage(WebView2 webView)
        {
            if (webView.CoreWebView2 == null)
            {
                return new Dictionary<string, string>();
            }

            try
            {
                var localStorageScript = "JSON.stringify(localStorage);";
                var localStorageData = await webView.CoreWebView2.ExecuteScriptAsync(localStorageScript);

                return localStorageData.JsonStringToDictionary<string, string>();
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        public static async Task<Dictionary<string, string>> GetBrowserSessionStorage(WebView2 webView)
        {
            if (webView.CoreWebView2 == null)
            {
                return new Dictionary<string, string>();
            }

            try
            {
                var localStorageScript = "JSON.stringify(sessionStorage);";
                var localStorageData = await webView.CoreWebView2.ExecuteScriptAsync(localStorageScript);

                return localStorageData.JsonStringToDictionary<string, string>();
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        public static string GetBrowserUserAgent(WebView2 webView)
        {
            if (webView.CoreWebView2 == null)
            {
                return null;
            }

            try
            {
                return webView.CoreWebView2.Settings.UserAgent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async void ScrollDown(WebView2 webView, int verticalPixels = 0)
        {
            if (webView.CoreWebView2 == null)
            {
                return;
            }

            try
            {
                var scrollTo = verticalPixels == 0 ? "document.body.scrollHeight" : verticalPixels.ToString();
                var script = $"window.scrollTo(0, {scrollTo});";
                await webView.CoreWebView2.ExecuteScriptAsync(script);

                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async void ScrollDownAllDivOnPage(WebView2 webView, TimeSpan? wait = null)
        {
            if (webView.CoreWebView2 == null)
            {
                return;
            }

            try
            {
                var filePath = Path.Combine(ScriptDirectory, "ScrollDownAllDivOnPage.js");
                var script = System.IO.File.ReadAllText(filePath);
                await webView.CoreWebView2.ExecuteScriptAsync(script);
                await webView.CoreWebView2.ExecuteScriptAsync($"scrollDownAllDivOnPage({wait?.TotalSeconds ?? 2})");
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async void ScrollUp(WebView2 webView, int verticalPixels = 0)
        {
            if (webView.CoreWebView2 == null)
            {
                return;
            }

            try
            {
                var script = $"window.scrollTo(0, {verticalPixels});";
                await webView.CoreWebView2.ExecuteScriptAsync(script);

                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async void HumanClick(WebView2 webView)
        {
            if (webView.CoreWebView2 == null)
            {
                return;
            }

            try
            {
                var filePath = Path.Combine(ScriptDirectory, "HumanClick.js");
                var script = System.IO.File.ReadAllText(filePath);
                await webView.CoreWebView2.ExecuteScriptAsync(script);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }


        private static EventHandler<CoreWebView2WebMessageReceivedEventArgs> handler;

        public static async Task<(bool, string)> ExecuteScriptForResult(WebView2 webView, string script)
        {
            if (webView.CoreWebView2 == null) return (false, "WebView2 is null");

            var tcs = new TaskCompletionSource<(bool, string)>();

            handler = (s, e) =>
            {
                var result = e.WebMessageAsJson;
                webView.CoreWebView2.WebMessageReceived -= handler;
                tcs.TrySetResult((true, result));
            };

            webView.CoreWebView2.WebMessageReceived += handler;

            await webView.ExecuteScriptAsync(script);

            return await tcs.Task;
        }
    }
}
