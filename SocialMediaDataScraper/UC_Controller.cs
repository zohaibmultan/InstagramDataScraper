#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace SocialMediaDataScraper
{
    public partial class UC_Controller : UserControl
    {
        public CancellationTokenSource cancellationToken;

        private BindingList<DS_BrowserLog> logs { get; set; } = [];
        private List<DS_BrowserTask> taskList { get; set; } = [];

        private DS_Browser dsBrowser { get; set; }
        private WebView2 webView { get; set; }
        private InstaHelper instaHelper { get; set; }

        public UC_Controller()
        {
            InitializeComponent();

            instaHelper = new InstaHelper();
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

        public void SetTaskList(List<DS_BrowserTask> list)
        {
            taskList = list;
        }

        public async void StartTasks()
        {
            foreach (var task in taskList)
            {
                logs.Clear();
                listBox.SafeInvoke(() => listBox.Refresh());

                switch (task.QueryData)
                {
                    case QueryProfile query:
                        await StartSingleTask(task.QueryAction, query);
                        break;

                    case QuerySinglePost query:
                        await StartSingleTask(task.QueryAction, query);
                        break;

                    case QueryBulkPosts query:
                        await StartSingleTask(task.QueryAction, query);
                        break;

                    case QueryFollowing query:
                        await StartSingleTask(task.QueryAction, query);
                        break;

                    case QueryFollowingAjax query:
                        await StartSingleTask(task.QueryAction, query);
                        break;

                    case QueryPostComments query:
                        await StartSingleTask(task.QueryAction, query);
                        break;
                }

                task.IsDone = true;
                task.DoneAt = DateTime.Now;
                task.DoneBy = dsBrowser.Username;
                task.Logs = [.. logs.Select(x => x.Text)];

                DbHelper.UpdateOne(task);
            }
        }

        private async Task StartSingleTask<T>(string command, T query)
        {
            if (dsBrowser.IsTaskRunning || string.IsNullOrEmpty(command)) return;

            void UpdateUI(bool enable)
            {
                cb_commands.SafeInvoke(() => cb_commands.Enabled = enable);
                btn_runCommand.SafeInvoke(() => btn_runCommand.Enabled = enable);
                btn_stopCommand.SafeInvoke(() => btn_stopCommand.Enabled = !enable);
            }

            try
            {
                UpdateUI(false);
                dsBrowser.IsTaskRunning = true;

                switch (command)
                {
                    case QueryAction.RecheckLoginStatus:
                        await RecheckLoginStatus();
                        break;

                    case QueryAction.GetUserProfile:
                        await GetUserProfile(query as QueryProfile);
                        break;

                    case QueryAction.GetSinglePost:
                        await GetSinglePost(query as QuerySinglePost);
                        break;

                    case QueryAction.GetPostsByUser:
                        await GetPostsByUser(query as QueryBulkPosts);
                        break;

                    case QueryAction.GetFollowings:
                        await GetFollowings(query as QueryFollowing);
                        break;

                    case QueryAction.GetFollowingsAjax:
                        await GetFollowingsAjax(query as QueryFollowingAjax);
                        break;

                    case QueryAction.GetPostComments:
                        await GetPostComments(query as QueryPostComments);
                        break;

                    case QueryAction.MonitorFollowRequest:
                        await MonitorFollowRequest();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.GetAllInnerMessages().ForEach(x => Log(DS_BrowserLogType.Error, x));
            }
            finally
            {
                UpdateUI(true);
                cb_commands.SafeInvoke(() => cb_commands.SelectedItem = QueryAction.NoAction);
                dsBrowser.IsTaskRunning = false;
            }
        }


        public long Log(string type, string message, long? logId = null, bool replace = false, string content = null)
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
                    log.Content = content ?? log.Content;
                    logs.ResetItem(index);
                }

                return log.ID;
            }
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

        private async Task RecheckLoginStatus()
        {
            var logId = Log(DS_BrowserLogType.Info, "Checking user is login...");

            var res = await instaHelper.TestLogin(webView, dsBrowser.Username);
            if (!res.Status)
            {
                Log(DS_BrowserLogType.Error, "Failed", logId, true);
                dsBrowser.IsLogin = false;
                return;
            }

            dsBrowser.IsLogin = true;
            Log(DS_BrowserLogType.Info, "OK", logId, true);
        }

        private async Task GetUserProfile(QueryProfile query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryProfile>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            Log(DS_BrowserLogType.Info, $"-------- GET PROFILE --------");

            InstaResult<InstaProfile> data = null;
            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.Username}...");
                data = await instaHelper.GetProfileByUsername(webView, query.Username);
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting profile {query.ProfileUrl}...");
                data = await instaHelper.GetProfileByUrl(webView, query.ProfileUrl);
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

        private async Task GetSinglePost(QuerySinglePost query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QuerySinglePost>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            Log(DS_BrowserLogType.Info, $"-------- GET POST --------");

            InstaResult<InstaPostVr2> data = null;
            if (!string.IsNullOrEmpty(query.PostShortCode))
            {
                Log(DS_BrowserLogType.Info, $"Getting post {query.PostShortCode}...");
                data = await instaHelper.GetPostByShortCode(webView, query.PostShortCode);
            }
            else if (!string.IsNullOrEmpty(query.PostUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting post {query.PostUrl}...");
                data = await instaHelper.GetPostByUrl(webView, query.PostUrl);
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

        private async Task GetPostsByUser(QueryBulkPosts query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryBulkPosts>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            InstaResult<List<InstaPost>> data = null;
            cancellationToken = new CancellationTokenSource();

            void TaskCancel(object sender, EventArgs e)
            {
                var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;
                cancellationToken.Cancel();
            }

            Log(DS_BrowserLogType.Info, $"-------- GET POSTS --------");
            btn_stopCommand.Click += TaskCancel;

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting posts {query.Username}...");
                data = await instaHelper.GetPostsByUsername(
                    query.Username,
                    new InstaBulkTaskParams<InstaPost>()
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
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting posts {query.ProfileUrl}...");
                data = data = await instaHelper.GetPostsByUrl(
                    query.ProfileUrl,
                    new InstaBulkTaskParams<InstaPost>()
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

        private async Task GetFollowings(QueryFollowing query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryFollowing>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => CancelRunningTask(cancellationToken);

            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS --------");
            btn_stopCommand.Click += canellationEvent;

            if (!string.IsNullOrEmpty(query.Username))
            {
                Log(DS_BrowserLogType.Info, $"Getting followings {query.Username}...");
                var data = await instaHelper.GetFollowingsByUsername(
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
            }
            else if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                Log(DS_BrowserLogType.Info, $"Getting followings {query.ProfileUrl}...");
                var data = await instaHelper.GetFollowingsByUrl(
                    query.ProfileUrl,
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
            }

            btn_stopCommand.Click -= canellationEvent;
            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS END --------");
        }

        private async Task GetFollowingsAjax(QueryFollowingAjax query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryFollowingAjax>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => CancelRunningTask(cancellationToken);

            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS --------");
            btn_stopCommand.Click += canellationEvent;

            Log(DS_BrowserLogType.Info, $"Getting followings {query.Username}...");
            var data = await instaHelper.GetFollowingsAjax(
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

        private async Task GetPostComments(QueryPostComments query = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryPostComments>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => CancelRunningTask(cancellationToken);

            Log(DS_BrowserLogType.Info, $"-------- GET POST COMMENTS --------");
            btn_stopCommand.Click += canellationEvent;

            Log(DS_BrowserLogType.Info, $"Getting post comments {query.PostShortCode}...");
            var data = await instaHelper.GetPostComments(
                query.PostShortCode,
                new InstaBulkTaskParams<InstaComment>()
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
            SaveData<List<InstaComment>, InstaComment>(data);

            btn_stopCommand.Click -= canellationEvent;
            Log(DS_BrowserLogType.Info, $"-------- GET POST COMMENTS END --------");
        }

        private async Task MonitorFollowRequest()
        {
            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => CancelRunningTask(cancellationToken);
            btn_stopCommand.Click += canellationEvent;

            Log(DS_BrowserLogType.Info, $"-------- MONITORING STARTED --------");
            await instaHelper.MonitorFollowRequest(new InstaBulkTaskParams<List<string>>()
            {
                WebView = webView,
                CancellationToken = cancellationToken,
                TaskProgress = async (s, e) =>
                {
                    var user_id = e.Message;
                    var logId = Log(DS_BrowserLogType.Info, $"{user_id} captured...", content: $"{user_id}");

                    await Task.Delay(new Random().Next(3, 10) * 1000);

                    var username = await instaHelper.GetUsernameByUserPk(user_id);
                    StaticInfo.CreateTasksFromUserId(user_id, username);
                    Log(DS_BrowserLogType.Info, $"Task created {username}", logId, true, $"{user_id} - {username}");
                },
            });
            Log(DS_BrowserLogType.Info, $"-------- MONITORING ENDED --------");

            btn_stopCommand.Click -= canellationEvent;
            cancellationToken.Dispose();
        }

        private void BreakLoop(TimeSpan time)
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

            var remainingTime = time.Seconds;
            var ticker = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };
            ticker.Elapsed += (s, e) =>
            {
                form.SafeInvoke(() => form.Text = title + (remainingTime -= 1) + " seconds");
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

        private void UC_Controller_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
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
            var command = cb_commands.SelectedItem as string;
            await StartSingleTask(command);
        }

        private async Task StartSingleTask(string command)
        {
            if (dsBrowser.IsTaskRunning || string.IsNullOrEmpty(command)) return;

            void UpdateUI(bool enable)
            {
                cb_commands.SafeInvoke(() => cb_commands.Enabled = enable);
                btn_runCommand.SafeInvoke(() => btn_runCommand.Enabled = enable);
                btn_stopCommand.SafeInvoke(() => btn_stopCommand.Enabled = !enable);
            }

            try
            {
                UpdateUI(false);
                dsBrowser.IsTaskRunning = true;

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
                        await GetFollowings();
                        break;

                    case QueryAction.GetFollowingsAjax:
                        await GetFollowingsAjax();
                        break;

                    case QueryAction.GetPostComments:
                        await GetPostComments();
                        break;

                    case QueryAction.MonitorFollowRequest:
                        await MonitorFollowRequest();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.GetAllInnerMessages().ForEach(x => Log(DS_BrowserLogType.Error, x));
            }
            finally
            {
                UpdateUI(true);
                cb_commands.SafeInvoke(() => cb_commands.SelectedItem = QueryAction.NoAction);
                dsBrowser.IsTaskRunning = false;
            }
        }
    }
}
