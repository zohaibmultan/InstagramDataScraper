#nullable disable

using SocialMediaDataScraper.Models;

namespace SocialMediaDataScraper
{
    public partial class FormDsBrowser : Form
    {
        DS_Browser dsBrowser;
        UC_Controller uc_controller;
        UC_WebView uc_webView;

        public FormDsBrowser(DS_Browser browser)
        {
            InitializeComponent();
            dsBrowser = browser;

            uc_webView = new UC_WebView()
            {
                Dock = DockStyle.Fill,
            };
            uc_controller = new UC_Controller()
            {
                Dock = DockStyle.Fill
            };

            splitContainer.Panel1.Controls.Add(uc_webView);
            splitContainer.Panel2.Controls.Add(uc_controller);
        }

        private async void InitializeUserControls()
        {
            var logId = uc_controller.Log(DS_BrowserLogType.Info, "Browser instance initizlizing...");

            await uc_webView.SetBrowserData(dsBrowser);
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            while (!token.IsCancellationRequested && !uc_webView.IsWebViewReady)
            {
                uc_controller.Log(DS_BrowserLogType.Info, "...", logId, true);
                await Task.Delay(5000);
            }

            if (uc_webView.IsWebViewReady)
            {
                uc_controller.Log(DS_BrowserLogType.Info, "OK", logId, true);
                uc_controller.SetBrowserData(dsBrowser, uc_webView.WebView);
            }
            else
            {
                uc_controller.Log(DS_BrowserLogType.Error, "Failed", logId, true);
            }
        }

        private void FormDsBrowser_Shown(object sender, EventArgs e)
        {
            InitializeUserControls();
        }
    }
}
