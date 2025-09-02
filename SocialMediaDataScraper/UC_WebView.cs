#nullable disable

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;

namespace SocialMediaDataScraper
{
    public partial class UC_WebView : UserControl
    {
        private DS_Browser dsBrowser { get; set; }
        public bool IsWebViewReady = false;
        public WebView2 WebView
        {
            get
            {
                return webView;
            }
        }

        public UC_WebView()
        {
            InitializeComponent();
        }

        public async Task SetBrowserData(DS_Browser browser)
        {
            dsBrowser = browser;
            await InitializeWebView();
        }

        private async Task InitializeWebView()
        {
            if (dsBrowser == null) return;

            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                IsWebViewReady = e.IsSuccess;
                if (e.IsSuccess)
                {
                    webView.CoreWebView2.DocumentTitleChanged += (s1, e1) =>
                    {
                        Text = webView.CoreWebView2.DocumentTitle;
                    };
                    webView.CoreWebView2.SourceChanged += (s1, e1) =>
                    {
                        tb_addressBar.SetTextSafe(webView.CoreWebView2.Source);
                    };
                    webView.CoreWebView2.NavigationStarting += (s1, e1) =>
                    {
                        tb_status.SetTextSafe("Loading...");
                    };
                    webView.CoreWebView2.NavigationCompleted += (s1, e1) =>
                    {
                        tb_status.SetTextSafe("Done");
                    };
                }
            };

            var environment = await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: Path.Combine(StaticInfo.UserSessionDirectory, dsBrowser.Username),
                options: new CoreWebView2EnvironmentOptions()
            );

            await webView.EnsureCoreWebView2Async(environment);
            webView.ZoomFactor = 1;
            webView.CoreWebView2.Settings.UserAgent = dsBrowser.UserAgent;
            //webView.CoreWebView2.Settings.UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 18_6_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1";
            webView.Source = new Uri("https://www.instagram.com");
        }

        private void UC_WebView_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
        }

        private void btn_go_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null || string.IsNullOrWhiteSpace(tb_addressBar.Text.Trim())) return;
            webView.Source = new Uri(StaticInfo.NormalizeUrl(tb_addressBar.Text.Trim().ToLower()));
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null) return;
            webView.CoreWebView2.Reload();
        }

        private void btn_devTool_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null) return;
            webView.CoreWebView2.OpenDevToolsWindow();
        }

        private void btn_zoomIn_Click(object sender, EventArgs e)
        {
            webView.ZoomFactor += 0.10;
        }

        private void btn_zoomOut_Click(object sender, EventArgs e)
        {
            webView.ZoomFactor -= 0.10;
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null || !webView.CoreWebView2.CanGoBack) return;
            webView.CoreWebView2.GoBack();
        }

        private void btn_forward_Click(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null || !webView.CoreWebView2.CanGoForward) return;
            webView.CoreWebView2.GoForward();
        }
    }
}
