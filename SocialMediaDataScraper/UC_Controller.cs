#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.Collections;
using System.ComponentModel;
using System.Threading;

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

            if (new PropertyForm("Task Details", query).ShowDialog() != DialogResult.OK) return;

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

                var model = DbHelper.SaveOne<InstaProfile>(data.Content);
                Log(model != null ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, model != null ? "Profile data saved" : "Unable to save profile data");
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

            if (new PropertyForm("Task Details", query).ShowDialog() != DialogResult.OK) return;

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

                var model = DbHelper.SaveOne<InstaPostVr2>(data.Content);
                Log(model != null ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, model != null ? "Post data saved" : "Unable to save post data");
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
            var query = new QueryBulkPosts();
            InstaResult<List<InstaPost>> data = null;
            var cancellationToken = new CancellationTokenSource();

            void TaskCancel(object sender, EventArgs e)
            {
                var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;
                cancellationToken.Cancel();
            }

            if (new PropertyForm("Task Details", query).ShowDialog() != DialogResult.OK) return;

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
                    query.RecordsCount,
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
                    query.RecordsCount,
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

        private async Task GetFollowings()
        {
            var query = new QueryBulkPosts();
            InstaResult<List<InstaFollowing>> data = null;
            var cancellationToken = new CancellationTokenSource();

            void TaskCancel(object sender, EventArgs e)
            {
                var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;
                cancellationToken.Cancel();
            }

            if (new PropertyForm("Task Details", query).ShowDialog() != DialogResult.OK) return;

            if (webView == null || webView.CoreWebView2 == null) return;

            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS --------");
            btn_stopCommand.Click += TaskCancel;

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting followings {query.Username}...");
                data = await InstaHelper.GetFollowingsByUsername(
                    webView,
                    query.Username,
                    cancellationToken,
                    query.RecordsCount,
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
                Log(DS_BrowserLogType.Info, $"Getting followings {query.ProfileUrl}...");
                data = data = await InstaHelper.GetFollowingsByUsername(
                    webView,
                    query.ProfileUrl,
                    cancellationToken,
                    query.RecordsCount,
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
                Log(DS_BrowserLogType.Info, $"Following data collected, Double click to view", content: jsonData);

                var ans = DbHelper.SaveMany<InstaFollowing>(data.Content);
                Log(ans ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, ans ? "Following data saved" : "Unable to save following data");
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
            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS END --------");
        }

        private async Task GetFollowingsAjax()
        {
            var cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => CancelRunningTask(cancellationToken);
            var (res, query) = ShowQueryDialog<QueryFollowingAjax>();
            if (res != DialogResult.OK) return;

            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS --------");
            btn_stopCommand.Click += canellationEvent;

            Log(DS_BrowserLogType.Info, $"Getting followings {query.Username}...");
            var data = await InstaHelper.GetFollowingsAjax(
                query.UserPK.ToString(),
                query.Username,
                new InstaBulkTaskParams<InstaFollowing>()
                {
                    WebView = webView,
                    CancellationToken = cancellationToken,
                    RecordsCount = query.RecordsCount,
                    MinWait = query.MinWait,
                    MaxWait = query.MaxWait,
                    TaskProgress = (s, e) =>
                    {
                        Log(DS_BrowserLogType.Info, e.Message);
                        if (e.BreakLoop) BreakLoop(e.BreakLoopWait);
                    },
                    LoopBreakAttempts = query.LoopBreak,
                    FailedAttempts = 3,
                }
            );
            SaveData<List<InstaFollowing>, InstaFollowing>(data);

            btn_stopCommand.Click -= canellationEvent;
            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS END --------");
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
                form.SafeInvoke(() => form.Text = title + (remainingTime -= 1000) / 1000 + " seconds");
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

                    case QueryAction.GetFollowings:
                        await GetFollowingsAjax();
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

        }

        private (DialogResult, T) ShowQueryDialog<T>() where T : class, new()
        {
            var model = Activator.CreateInstance<T>();
            var result = new PropertyForm("Task Details", model).ShowDialog();
            return (result, model);
        }

        private void SaveData<T1, T2>(InstaResult<T1> data) where T1 : class where T2 : class
        {
            if (data == null)
            {
                Log(DS_BrowserLogType.Error, "Collected data is null");
                return;
            }

            if (!data.Status)
            {
                data.Errors?.ForEach(error => Log(DS_BrowserLogType.Error, error));
                return;
            }

            var jsonData = JsonConvert.SerializeObject(data.Content, Formatting.Indented);
            Log(DS_BrowserLogType.Info, $"Following data collected, Double click to view", content: jsonData);

            var success = false;
            if (data.Content is IList list)
            {
                var castedList = list.Cast<T2>().ToList();
                success = DbHelper.SaveMany<T2>(castedList);
            }
            else if (data.Content is T1 obj)
            {
                var res = DbHelper.SaveOne(obj);
                success = res != null;
            }
            else
            {
                Log(DS_BrowserLogType.Error, "Data content type is not supported");
                return;
            }

            Log(success ? DS_BrowserLogType.Info : DS_BrowserLogType.Error, success ? "Following data saved" : "Unable to save following data");
        }

        private void CancelRunningTask(CancellationTokenSource cancellationToken)
        {
            var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;
            cancellationToken.Cancel();
        }
    }
}
