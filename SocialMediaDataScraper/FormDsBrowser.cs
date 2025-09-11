#nullable disable

using SocialMediaDataScraper.Models;

namespace SocialMediaDataScraper
{
    public partial class FormDsBrowser : Form
    {
        public DS_UserAccount userAccount;
        public UC_WebView uc_webView;
        public UC_Controller uc_controller;
        public bool forceClose;

        public FormDsBrowser(DS_UserAccount account)
        {
            InitializeComponent();
            Height = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.98);
            userAccount = account;
            uc_webView = new UC_WebView(account)
            {
                Dock = DockStyle.Fill,
            };
            uc_controller = new UC_Controller(account, uc_webView.WebView)
            {
                Dock = DockStyle.Fill
            };

            splitContainer.Panel1.Controls.Add(uc_webView);
            splitContainer.Panel2.Controls.Add(uc_controller);

            Text = account.Username + " | " + account.Email;
        }

        private async void Initialize()
        {
            var logId = uc_controller.Log(DS_BrowserLogType.Info, "Browser instance initizlizing...");
            var (status, errors) = await uc_webView.Initialize();
            if (!status)
            {
                uc_controller.Log(DS_BrowserLogType.Error, "Browser instance initizlizing failed", logId, true, string.Join("\n", errors));
                return;
            }

            uc_controller.Log(DS_BrowserLogType.Info, "OK", logId, true);
            uc_controller.Initialize();
        }

        #region Events
        private void FormDsBrowser_Shown(object sender, EventArgs e)
        {
            Initialize();
        }
        #endregion
    }
}
