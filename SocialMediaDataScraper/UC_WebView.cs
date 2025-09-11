#nullable disable

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;

namespace SocialMediaDataScraper
{
    public partial class UC_WebView : UserControl
    {
        private DS_UserAccount userAccount { get; set; }
        public bool IsWebViewReady = false;
        public WebView2 WebView
        {
            get => webView;
        }

        public UC_WebView(DS_UserAccount account)
        {
            InitializeComponent();
            userAccount = account;
        }

        public async Task<(bool, List<string>)> Initialize()
        {
            try
            {
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
                    userDataFolder: Path.Combine(StaticInfo.UserSessionDirectory, userAccount.Username),
                    options: new CoreWebView2EnvironmentOptions("--disable-web-security")
                );

                await webView.EnsureCoreWebView2Async(environment);

                webView.ZoomFactor = 1;
                webView.CoreWebView2.Settings.UserAgent = userAccount.UserAgent;
                webView.Source = new Uri("https://www.instagram.com");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.GetAllInnerMessages());
            }
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

        private void btn_store_Click(object sender, EventArgs e)
        {
            var url = tb_addressBar.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;

            StaticInfo.CreateTasksFromUrl(url);
        }
    }
}
