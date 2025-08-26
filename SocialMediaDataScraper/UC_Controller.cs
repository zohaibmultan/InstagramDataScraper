#nullable disable

using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace SocialMediaDataScraper
{
    public partial class UC_Controller : UserControl
    {
        private DS_Browser dsBrowser { get; set; }
        private BindingList<DS_BrowserLog> logs { get; set; } = new BindingList<DS_BrowserLog>();
        private WebView2 webView { get; set; }


        public UC_Controller()
        {
            InitializeComponent();

            listBox.DataSource = logs;
            listBox.DisplayMember = nameof(DS_BrowserLog.Text);
            listBox.ValueMember = nameof(DS_BrowserLog.ID);
            cb_commands.DataSource = new BindingList<string>(QueryAction.GetAllQueryActions());
        }

        public void SetBrowserData(DS_Browser browser, WebView2 webview)
        {
            dsBrowser = browser;
            webView = webview;
            tb_username.Text = browser.Username;

            _ = RecheckLoginStatus();
        }

        public long Log(string type, string message, long? logId = null, bool replace = false, string? content = null)
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

            var res = await InstaHelper.TestLogin(webView, dsBrowser.Username);
            if (!res.Status)
            {
                Log(DS_BrowserLogType.Error, "Failed", logId, true);
                return;
            }

            Log(DS_BrowserLogType.Info, "OK", logId, true);
        }

        private async Task GetUserProfile()
        {
            var query = new QueryProfile();
            InstaResult<InstaProfile> data = null;

            if (new PropertyForm("Enter the Profile Details", query).ShowDialog() != DialogResult.OK) return;

            if (webView == null || webView.CoreWebView2 == null) return;

            Log(DS_BrowserLogType.Info, $"-------- GET PROFILE --------");

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.Username}...");
                data = await InstaHelper.GetProfileByUsername(webView, query.Username);
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.ProfileUrl}...");
                data = await InstaHelper.GetProfileByUrl(webView, query.ProfileUrl);
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

        private async Task GetSinglePost()
        {
            var query = new QuerySinglePost();
            InstaResult<InstaPostVr2> data = null;

            if (new PropertyForm("Enter the Post Details", query).ShowDialog() != DialogResult.OK) return;

            if (webView == null || webView.CoreWebView2 == null) return;

            Log(DS_BrowserLogType.Info, $"-------- GET POST --------");

            if (!string.IsNullOrEmpty(query.PostShortCode))
            {
                Log(DS_BrowserLogType.Info, $"Getting post {query.PostShortCode}...");
                data = await InstaHelper.GetPostByShortCode(webView, query.PostShortCode);
            }
            else if (!string.IsNullOrEmpty(query.PostUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting post {query.PostUrl}...");
                data = await InstaHelper.GetPostByUrl(webView, query.PostUrl);
            }

            if (data != null && data.Status)
            {
                var jsonData = JsonConvert.SerializeObject(data.Content, Formatting.Indented);
                Log(DS_BrowserLogType.Info, $"Post data collected, Double click to view", content: jsonData);

                var ans = DbHelper.Save<InstaPostVr2>(data.Content);
                Log(ans ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, ans ? "Post data saved" : "Unable to save post data");
            }
            else if (data != null && !data.Status)
            {
                data.Errors.ForEach(error => Log(DS_BrowserLogType.Error, error));
            }
            else
            {
                Log(DS_BrowserLogType.Error, "Collected data is null");
            }

            Log(DS_BrowserLogType.Info, $"-------- GET POST END --------");
        }

        private async Task GetPostsByUser()
        {
            var query = new QueryBulkPost();
            InstaResult<List<InstaPost>> data = null;
            var cancellationToken = new CancellationTokenSource();

            void TaskCancel(object sender, EventArgs e)
            {
                var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;
                cancellationToken.Cancel();
            }

            if (new PropertyForm("Enter the Profile Details", query).ShowDialog() != DialogResult.OK) return;

            if (webView == null || webView.CoreWebView2 == null) return;

            Log(DS_BrowserLogType.Info, $"-------- GET POSTS --------");
            btn_stopCommand.Click += TaskCancel;

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting posts {query.Username}...");
                data = await InstaHelper.GetPostsByUsername(
                    webView,
                    query.Username,
                    cancellationToken,
                    query.NumberOfPosts,
                    query.MinWait,
                    query.MaxWait,
                    (s, e) =>
                    {
                        Log(DS_BrowserLogType.Info, e.Message);
                        if (e.BreakLoop) BreakLoop(e.BreakLoopWait);
                    },
                    query.LoopBreak
                );
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting posts {query.ProfileUrl}...");
                data = data = await InstaHelper.GetPostsByUrl(
                    webView,
                    query.ProfileUrl,
                    cancellationToken,
                    query.NumberOfPosts,
                    query.MinWait,
                    query.MaxWait,
                    (s, e) =>
                    {
                        Log(DS_BrowserLogType.Info, e.Message);
                        if (e.BreakLoop) BreakLoop(e.BreakLoopWait);
                    },
                    query.LoopBreak
                );
            }

            if (data != null && data.Status)
            {
                var jsonData = JsonConvert.SerializeObject(data.Content, Formatting.Indented);
                Log(DS_BrowserLogType.Info, $"Posts data collected, Double click to view", content: jsonData);

                var ans = DbHelper.SaveMany<InstaPost>(data.Content);
                Log(ans ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, ans ? "Posts data saved" : "Unable to save posts data");
            }
            else if (data != null && !data.Status)
            {
                data.Errors.ForEach(error => Log(DS_BrowserLogType.Error, error));
            }
            else
            {
                Log(DS_BrowserLogType.Error, "Collected data is null");
            }

            btn_stopCommand.Click -= TaskCancel;
            Log(DS_BrowserLogType.Info, $"-------- GET POSTS END --------");
        }

        private void BreakLoop(int time)
        {
            var title = dsBrowser.Username + " - Loop Breaker - Close in ";
            var uc_webView = new UC_WebView()
            {
                Dock = DockStyle.Fill,
            };
            var form = new Form()
            {
                Text = dsBrowser.Username + " - Loop Breaker",
                Width = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.6),
                Height = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9),
                StartPosition = FormStartPosition.CenterParent,
            };
            form.Controls.Add(uc_webView);
            form.Shown += async (s, e) =>
            {
                await uc_webView.SetBrowserData(dsBrowser);
                await Task.Delay(2000);
            };

            var timer = new System.Timers.Timer(time)
            {
                AutoReset = false
            };
            timer.Elapsed += (s, e) =>
            {
                form.SafeInvoke(() => form.Close());
            };
            timer.Start();

            var remainingTime = time;
            var ticker = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };
            ticker.Elapsed += (s, e) =>
            {
                form.SafeInvoke(() => form.Text = title + (remainingTime-=1000)/1000 + " seconds");
            };
            ticker.Start();

            form.Show();
            form.FormClosing += (s, e) =>
            {
                timer.Stop();
                ticker.Stop();
                timer.Dispose();
                ticker.Dispose();
            };
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            var item = listBox.SelectedItem as DS_BrowserLog;
            if (item == null || string.IsNullOrEmpty(item.Content)) return;

            var textBox = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Font = listBox.Font,
                Text = item.Content,
                ReadOnly = true,
            };

            var form = new Form();
            form.Controls.Add(textBox);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.8);
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
                cb_commands.SafeInvoke(() => cb_commands.Enabled = enable);
                btn_runCommand.SafeInvoke(() => btn_runCommand.Enabled = enable);
                btn_stopCommand.SafeInvoke(() => btn_stopCommand.Enabled = !enable);
            }

            try
            {
                UpdateUI(false);
                var command = cb_commands.SelectedItem as string;
                cb_commands.SelectedItem = QueryAction.NoAction;

                switch (command)
                {
                    case QueryAction.RecheckLoginStatus:
                        await RecheckLoginStatus();
                        break;

                    case QueryAction.GetUserProfile:
                        await GetUserProfile();
                        break;

                    case QueryAction.GetSinglePost:
                        await GetSinglePost();
                        break;

                    case QueryAction.GetPostsByUser:
                        await GetPostsByUser();
                        break;

                    case QueryAction.GetPostComments:
                        break;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                UpdateUI(true);
            }
        }

        private void UC_Controller_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            BreakLoop(15000);
        }
    }
}
