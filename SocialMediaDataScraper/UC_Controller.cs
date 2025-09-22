#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialMediaDataScraper
{
    public partial class UC_Controller : UserControl
    {
        public CancellationTokenSource cancellationToken;

        private BindingList<DS_BrowserLog> logs = [];
        private DS_UserAccount userAccount;
        private WebView2 webView;
        private InstaHelper instaHelper;


        public UC_Controller(DS_UserAccount account, WebView2 webview)
        {
            InitializeComponent();

            instaHelper = new InstaHelper();
            userAccount = account;
            webView = webview;
            tb_username.Text = account.Username;

            listBox.DataSource = logs;
            listBox.DisplayMember = nameof(DS_BrowserLog.Text);
            listBox.ValueMember = nameof(DS_BrowserLog.ID);

            cb_commands.DataSource = new BindingList<string>(QueryAction.GetAllQueryActions());

            gv_tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_tasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv_tasks.MultiSelect = false;
            gv_tasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gv_tasks.ColumnHeadersHeight = 30;
            gv_tasks.RowTemplate.Height = 30;
            gv_tasks.ReadOnly = true;
            gv_tasks.AllowUserToAddRows = false;
            gv_tasks.AllowUserToDeleteRows = false;
            gv_tasks.AllowUserToResizeRows = false;
            gv_tasks.RowHeadersVisible = false;
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

        private void BreakLoop(TimeSpan time)
        {
            var title = userAccount.Username + " - Loop Breaker - Close in ";
            var uc_webView = new UC_WebView(userAccount)
            {
                Dock = DockStyle.Fill,
            };
            var form = new Form()
            {
                Text = userAccount.Username + " - Loop Breaker",
                Width = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.6),
                Height = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9),
                StartPosition = FormStartPosition.CenterParent,
            };
            form.Controls.Add(uc_webView);
            form.Shown += async (s, e) =>
            {
                await uc_webView.Initialize();
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

        public void Initialize()
        {
            _ = RecheckLoginStatus();
        }

        public async Task StartTasks(List<DS_BrowserTask> taskList)
        {
            if (cancellationToken != null)
            {
                cancellationToken?.Dispose();
                cancellationToken = null;
            }

            gv_tasks.DataSource = taskList;
            gv_tasks.Columns[nameof(DS_BrowserTask.QueryData)].Visible = false;
            gv_tasks.Columns[nameof(DS_BrowserTask.QueryObjectType)].Visible = false;
            gv_tasks.Columns[nameof(DS_BrowserTask.DoneBy)].Visible = false;
            gv_tasks.Refresh();

            foreach (var task in taskList)
            {
                if (cancellationToken?.IsCancellationRequested == true) break;

                logs.Clear();
                listBox.SafeInvoke(() => listBox.Refresh());

                var result = await StartSingleTask(task.QueryAction, task.QueryData, task.ID);

                task.Status = result ? DS_BrowserTask_Status.Done : DS_BrowserTask_Status.Error;
                task.DoneAt = DateTime.Now;
                task.DoneBy = userAccount.Username;
                task.Logs = [.. logs.Select(x => x.Text)];

                DbHelper.UpdateOne(task);

                if (cancellationToken?.IsCancellationRequested == true) break;

                gv_tasks.Refresh();
                await Task.Delay(new Random().Next(5, 15));
            }
        }

        public async Task StopTasks()
        {
            cancellationToken?.Cancel();
            await Task.Delay(1000);
        }

        private async Task<bool> StartSingleTask<T>(string command, T query = null, ObjectId taskID = null) where T : class
        {
            if (userAccount.IsTaskRunning || string.IsNullOrEmpty(command)) return false;

            void UpdateUI(bool enable)
            {
                cb_commands.SafeInvoke(() => cb_commands.Enabled = enable);
                btn_runCommand.SafeInvoke(() => btn_runCommand.Enabled = enable);
                btn_stopCommand.SafeInvoke(() => btn_stopCommand.Enabled = !enable);
            }

            try
            {
                UpdateUI(false);
                userAccount.IsTaskRunning = true;

                switch (command)
                {
                    case QueryAction.RecheckLoginStatus:
                        await RecheckLoginStatus();
                        break;

                    case QueryAction.GetUserProfile:
                        await GetUserProfile(query as QueryProfile, taskID);
                        break;

                    case QueryAction.GetSinglePost:
                        await GetSinglePost(query as QuerySinglePost, taskID);
                        break;

                    case QueryAction.GetPostsByUser:
                        await GetPostsByUser(query as QueryBulkPosts, taskID);
                        break;

                    case QueryAction.GetFollowings:
                        await GetFollowings(query as QueryFollowing, taskID);
                        break;

                    case QueryAction.GetFollowingsAjax:
                        await GetFollowingsAjax(query as QueryFollowingAjax, taskID);
                        break;

                    case QueryAction.GetPostComments:
                        await GetPostComments(query as QueryPostComments, taskID);
                        break;

                    case QueryAction.MonitorFollowRequest:
                        await MonitorFollowRequest();
                        break;

                    case QueryAction.GetUserPkFromUsername:
                        await GetUserPkFromUsername(query as QueryProfile, taskID);
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.GetAllInnerMessages().ForEach(x => Log(DS_BrowserLogType.Error, x));
                return false;
            }
            finally
            {
                UpdateUI(true);
                cb_commands.SafeInvoke(() => cb_commands.SelectedItem = QueryAction.NoAction);
                userAccount.IsTaskRunning = false;
            }
        }

        private (DialogResult, T) ShowQueryDialog<T>() where T : class, new()
        {
            var model = Activator.CreateInstance<T>();
            var result = new PropertyForm("Task Details", model).ShowDialog();
            return (result, model);
        }

        private void SaveTaskData<T1, T2>(InstaResult<T1> data) where T1 : class where T2 : class
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

        private async Task<InstaResult<string>> RunPythonScript(string func, string param, int max = 0, int amt = 0)
        {
            try
            {
                var workingDir = StaticInfo.AppSetting.PythonScriptDirectory;
                var pythonExe = StaticInfo.AppSetting.PythonExeDirectory;
                var pythonScript = @$"{workingDir}\instagram_handler.py";
                var arguments = $"--func={func} --parm={param}";
                arguments += max == 0 ? "" : $" max={max}";
                arguments += amt == 0 ? "" : $" amt={amt}";

                if (!System.IO.File.Exists(pythonExe))
                {
                    throw new System.IO.FileNotFoundException($"Python executable not found at: {pythonExe}");
                }

                if (!System.IO.File.Exists(pythonScript))
                {
                    throw new System.IO.FileNotFoundException($"Python script not found at: {pythonScript}");
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"{pythonScript} {arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir
                };

                using var process = new Process { StartInfo = processStartInfo };
                var output = new StringBuilder();
                var error = new StringBuilder();
                var appendOutput = false;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        if (appendOutput)
                        {
                            output.AppendLine(e.Data);
                        }

                        if (e.Data == "--xx--xx--")
                        {
                            appendOutput = true;
                        }
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        error.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    return new()
                    {
                        Status = true,
                        Content = output.ToString()
                    };
                }
                else
                {
                    return new()
                    {
                        Status = false,
                        Content = null,
                        Errors = [error.ToString()]
                    };
                }
            }
            catch (Exception ex)
            {
                return new()
                {
                    Status = false,
                    Content = null,
                    Errors = ex.GetAllInnerMessages()
                };
            }
        }

        #region Commands
        private async Task RecheckLoginStatus()
        {
            var logId = Log(DS_BrowserLogType.Info, "Checking user is login...");

            var res = await instaHelper.TestLogin(webView, userAccount.Username);
            if (!res.Status)
            {
                Log(DS_BrowserLogType.Error, "Failed", logId, true);
                userAccount.IsLogin = false;
                return;
            }

            userAccount.IsLogin = true;
            tb_userPk.SetTextSafe(res.Content);
            Log(DS_BrowserLogType.Info, "OK", logId, true);
        }

        private async Task MonitorFollowRequest()
        {
            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => _ = StopTasks();
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

        private async Task GetUserProfile(QueryProfile query = null, ObjectId taskId = null)
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

                if (taskId != null) data.Content.taskId = taskId;
                var model = DbHelper.SaveOne(data.Content);

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

        private async Task GetUserProfilePython(QueryProfile query = null, ObjectId taskId = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryProfile>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            if (!string.IsNullOrEmpty(query.ProfileUrl))
            {
                query.Username = new Uri(query.ProfileUrl).AbsolutePath.Trim('/');
            }

            Log(DS_BrowserLogType.Info, $"-------- GET PROFILE --------");
            Log(DS_BrowserLogType.Info, $"Getting profile {query.Username}...");
            var data = await RunPythonScript("get_user", query.Username);

            if (data != null && data.Status)
            {
                Log(DS_BrowserLogType.Info, $"Profile data collected, Double click to view", content: data.Content);
                var root = JObject.Parse(data.Content);
                
                //if (taskId != null) root["taskId"] = taskId;
                var model = DbHelper.SaveOne(data.Content);

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

        private async Task GetSinglePost(QuerySinglePost query = null, ObjectId taskId = null)
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

                if (taskId != null) data.Content.taskId = taskId;
                var model = DbHelper.SaveOne(data.Content);

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

        // todo: jab post end ho jatay hy to system wait per laga rehata hy
        private async Task GetPostsByUser(QueryBulkPosts query = null, ObjectId taskId = null)
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

                if (taskId != null) data.Content.ForEach(x => x.taskId = taskId);
                var ans = DbHelper.SaveMany(data.Content);

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

        private async Task GetFollowings(QueryFollowing query = null, ObjectId taskId = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryFollowing>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => _ = StopTasks();

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

                if (taskId != null) data.Content.ForEach(x => x.taskId = taskId);
                SaveTaskData<List<InstaFollowing>, InstaFollowing>(data);
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

                if (taskId != null) data.Content.ForEach(x => x.taskId = taskId);
                SaveTaskData<List<InstaFollowing>, InstaFollowing>(data);
            }

            btn_stopCommand.Click -= canellationEvent;
            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS END --------");
        }

        private async Task GetFollowingsAjax(QueryFollowingAjax query = null, ObjectId taskId = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryFollowingAjax>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            if (string.IsNullOrEmpty(query.Username) && query.UserPK != 0)
            {
                query.Username = await instaHelper.GetUsernameByUserPk(query.UserPK.ToString());
            }

            if (query.UserPK == 0 && !string.IsNullOrEmpty(query.Username))
            {
                var result = await instaHelper.GetUserPkByUsername(webView, query.Username);
                if (result.Status)
                {
                    var ans = long.TryParse(result.Content, out long userPkLong);
                    query.UserPK = ans ? userPkLong : query.UserPK;
                }
            }

            if (query.UserPK == 0)
            {
                Log(DS_BrowserLogType.Error, $"UserPk is not available, {query.UserPK}");
                return;
            }

            bool isLoading = !string.IsNullOrEmpty(query.Username);
            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => _ = StopTasks();
            EventHandler<CoreWebView2NavigationCompletedEventArgs> navigationComplete = null;
            navigationComplete = (s, e) =>
            {
                isLoading = false;
            };

            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS --------");
            btn_stopCommand.Click += canellationEvent;

            if (!string.IsNullOrEmpty(query.Username))
            {
                webView.Source = new Uri($"https://www.instagram.com/{query.Username}");
                webView.CoreWebView2.NavigationCompleted += navigationComplete;
                isLoading = true;
            }

            while (isLoading) await Task.Delay(1000);

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

            if (taskId != null) data.Content.ForEach(x => x.taskId = taskId);
            if (!string.IsNullOrEmpty(query.Username)) data.Content.ForEach(x => x.follower_username = query.Username);
            SaveTaskData<List<InstaFollowing>, InstaFollowing>(data);

            btn_stopCommand.Click -= canellationEvent;
            webView.CoreWebView2.NavigationCompleted -= navigationComplete;
            Log(DS_BrowserLogType.Info, $"-------- GET FOLLOWINGS END --------");
        }

        private async Task GetPostComments(QueryPostComments query = null, ObjectId taskId = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryPostComments>();
                if (res != DialogResult.OK) return;
                query = newQuery;
            }

            cancellationToken = new CancellationTokenSource();
            EventHandler canellationEvent = (sender, e) => _ = StopTasks();

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

            if (taskId != null) data.Content.ForEach(x => x.taskId = taskId);
            SaveTaskData<List<InstaComment>, InstaComment>(data);

            btn_stopCommand.Click -= canellationEvent;
            Log(DS_BrowserLogType.Info, $"-------- GET POST COMMENTS END --------");
        }

        private async Task<string> GetUserPkFromUsername(QueryProfile query = null, ObjectId taskId = null)
        {
            if (webView == null || webView.CoreWebView2 == null) return null;

            if (query == null)
            {
                var (res, newQuery) = ShowQueryDialog<QueryProfile>();
                if (res != DialogResult.OK) return null;
                query = newQuery;
            }

            try
            {
                Log(DS_BrowserLogType.Info, $"-------- GET USER PK --------");

                if (string.IsNullOrEmpty(query.Username))
                {
                    Log(DS_BrowserLogType.Error, $"Username is invalid, {query.Username}");
                    return null;
                }

                var result = await instaHelper.GetUserPkByUsername(webView, query.Username);
                if (!result.Status)
                {
                    result.Errors.ForEach(x => Log(DS_BrowserLogType.Error, x));
                    return null;
                }

                Log(DS_BrowserLogType.Info, $"User pk - {result.Content}");
                return result.Content;
            }
            catch (Exception ex)
            {
                ex.GetAllInnerMessages().ForEach(x => Log(DS_BrowserLogType.Error, x));
                return null;
            }
            finally
            {
                Log(DS_BrowserLogType.Info, $"-------- GET USER PK END --------");
            }
        }
        #endregion

        #region Events
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
                Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
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
            await StartSingleTask<DS_BrowserTask>(command);
        }

        private void btn_stopCommand_Click(object sender, EventArgs e)
        {

        }

        private void gv_tasks_DoubleClick(object sender, EventArgs e)
        {
            if (gv_tasks.SelectedRows.Count == 0) return;

            var task = gv_tasks.SelectedRows[0].DataBoundItem as DS_BrowserTask;
            var form = new TaskForm(task);
            var res = form.ShowDialog();
            if (res == DialogResult.OK) gv_tasks.Refresh();
        }
        #endregion
    }
}
