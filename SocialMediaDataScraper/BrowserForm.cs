#nullable disable

using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SocialMediaDataScraper
{
    public partial class BrowserForm : Form
    {
        private DS_Browser dsBrowser { get; set; }
        private BindingList<DS_BrowserLog> logs { get; set; } = new BindingList<DS_BrowserLog>();

        public BrowserForm(DS_Browser browser)
        {
            InitializeComponent();

            dsBrowser = browser;

            tb_username.Text = dsBrowser.Username;
            listBox.DataSource = logs;
            listBox.DisplayMember = nameof(DS_BrowserLog.Text);
            listBox.ValueMember = nameof(DS_BrowserLog.ID);
            cb_commands.DataSource = new BindingList<string>(QueryAction.GetAllQueryActions());
        }

        private long Log(string type, string message, long? logId = null, bool replace = false, string? content = null)
        {
            DS_BrowserLog GetNewLog()
            {
                return new DS_BrowserLog()
                {
                    ID = DateTime.Now.Ticks,
                    Message = message,
                    Text = $"{DateTime.Now:HH:mm:ss} - {type} - {message}",
                    Type = type,
                    Content = content,
                    CreatedAt = DateTime.Now,
                };
            }

            if (logId == null)
            {
                var log = GetNewLog();
                logs.Insert(0, log);
                return log.ID;
            }
            else
            {
                var log = logs.FirstOrDefault(x => x.ID == logId);
                if (log == null)
                {
                    log = GetNewLog();
                    logs.Insert(0, log);
                }

                if (replace)
                {
                    var index = logs.IndexOf(log);
                    log.Text = $"{DateTime.Now:HH:mm:ss} - {type} - {log.Message}{message}";
                    log.Type = type;
                    logs.ResetItem(index);
                }

                return log.ID;
            }
        }

        private async Task RecheckLoginStatus()
        {
            var logId = Log(DS_BrowserLogType.Info, "Checking user is login...");

            var res = await InstaHelper.TestLogin(uc_webView.WebView, dsBrowser.Username);
            if (!res.Status)
            {
                Log(DS_BrowserLogType.Error, "Failed", logId, true);
                return;
            }

            Log(DS_BrowserLogType.Info, "OK", logId, true);
        }

        private async void GetUserProfile()
        {
            var query = new QueryProfile();
            InstaResult<InstaProfile> data = null;

            if (new PropertyForm("Enter the Profile Details", query).ShowDialog() != DialogResult.OK) return;

            if (uc_webView.WebView == null || uc_webView.WebView.CoreWebView2 == null) return;

            Log(DS_BrowserLogType.Info, $"-------- GET PROFILE --------");

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.Username}...");
                data = await InstaHelper.GetProfileByUsername(uc_webView.WebView, query.Username);
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.ProfileUrl}...");
                data = await InstaHelper.GetProfileByUrl(uc_webView.WebView, query.ProfileUrl);
            }

            if (data != null && data.Status)
            {
                var jsonData = JsonConvert.SerializeObject(data.Content, Formatting.Indented);
                Log(DS_BrowserLogType.Info, $"Profile data collected, Double click to view", content: jsonData);

                var ans = DbHelper.Save<InstaProfile>(data.Content);
                Log(ans ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, ans ? "Profile data saved" : "Unable to save profile data");
            }
            else if (data != null && !data.Status)
            {
                data.Errors.ForEach(error => Log(DS_BrowserLogType.Error, error));
            }
            else
            {
                Log(DS_BrowserLogType.Error, "Collected data is null");
            }

            Log(DS_BrowserLogType.Info, $"-------- GET PROFILE END --------");
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            var item = listBox.SelectedItem as DS_BrowserLog;
            if (item == null || string.IsNullOrEmpty(item.Content)) return;

            var form = new Form();
            var textBox = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Font = listBox.Font,
                Text = item.Content,
                ReadOnly = true,
            };

            form.Controls.Add(textBox);
            form.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.7);
            form.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.7);
            form.Text = item.ID.ToString();
            form.ShowDialog();
        }

        private async void btn_runCommand_Click(object sender, EventArgs e)
        {
            void UpdateUI(bool enable)
            {
                btn_start.SafeInvoke(() => btn_start.Enabled = enable);
                btn_stop.SafeInvoke(() => btn_stop.Enabled = enable);
                btn_runCommand.SafeInvoke(() => btn_runCommand.Enabled = enable);
                cb_commands.SafeInvoke(() => cb_commands.Enabled = enable);
            }

            try
            {
                UpdateUI(false);
                var command = cb_commands.SelectedItem as string;
                switch (command)
                {
                    case QueryAction.RecheckLoginStatus:
                        await RecheckLoginStatus();
                        break;

                    case QueryAction.GetUserProfile:
                        GetUserProfile();
                        break;

                    case QueryAction.GetSinglePost:
                        break;
                    case QueryAction.GetUserAllPosts:
                        break;
                    case QueryAction.GetPostComments:
                        break;
                }

                cb_commands.SelectedItem = QueryAction.NoAction;
            }
            catch (Exception)
            {

            }
            finally
            {
                UpdateUI(true);
            }
        }

        private void BrowserForm_Shown(object sender, EventArgs e)
        {
            _ = uc_webView.SetBrowser(dsBrowser);
        }
    }
}
