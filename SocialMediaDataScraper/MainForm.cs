#nullable disable

using LiteDB;
using Newtonsoft.Json.Linq;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;
using System.Threading;

namespace SocialMediaDataScraper
{
    public partial class MainForm : Form
    {
        BindingList<DS_UserAccount> userAccounts = [];
        BindingList<DS_BrowserTask> browserTasks = [];
        Dictionary<string, FormDsBrowser> dsBrowsers = [];
        CancellationTokenSource bulkTaskCancelToken = new CancellationTokenSource();

        System.Timers.Timer downloadTimer;
        System.Timers.Timer downloadStatusTimer;
        bool isDownloading = false;
        static string downloadStatusText = string.Empty;
        static decimal downloadStatusWait = 0;

        public MainForm()
        {
            InitializeComponent();

            Width = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9);
            Height = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9);

            gv_browsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_browsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv_browsers.MultiSelect = true;
            gv_browsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gv_browsers.ColumnHeadersHeight = 30;
            gv_browsers.RowTemplate.Height = 30;
            gv_browsers.ReadOnly = true;
            gv_browsers.AllowUserToAddRows = false;
            gv_browsers.AllowUserToDeleteRows = false;
            gv_browsers.AllowUserToResizeRows = false;
            gv_browsers.RowHeadersVisible = false;

            gv_tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_tasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv_tasks.MultiSelect = true;
            gv_tasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gv_tasks.ColumnHeadersHeight = 30;
            gv_tasks.RowTemplate.Height = 30;
            gv_tasks.ReadOnly = true;
            gv_tasks.AllowUserToAddRows = false;
            gv_tasks.AllowUserToDeleteRows = false;
            gv_tasks.AllowUserToResizeRows = false;
            gv_tasks.RowHeadersVisible = false;

            tb_downlaodInterval.Minimum = 0;
            tb_downlaodInterval.Maximum = int.MaxValue;

            filter_status.Items.AddRange([.. DS_BrowserTask_Status.GetAllStatus()]);
            filter_query.Items.AddRange([.. QueryAction.GetAllQueryActions().Where(x => !string.IsNullOrEmpty(x))]);

            var accounts = DbHelper.GetAll<DS_UserAccount>().Select(x => x.Username).ToArray();
            filter_account.Items.AddRange(accounts);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            LoadAppSetting();
            LoadAccountsGrid();
            LoadTasksGrid();
            Cursor = Cursors.Default;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var ans = MessageBox.Show("Do you want to close?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) e.Cancel = true;

            if (downloadStatusTimer != null)
            {
                downloadStatusTimer.Stop();
                downloadStatusTimer?.Dispose();
            }

            if (downloadTimer != null)
            {
                downloadTimer.Stop();
                downloadTimer.Dispose();
            }

            foreach (var item in dsBrowsers)
            {
                _ = StopBrowser(item.Value);
            }
        }

        #region Account Tab
        void StartDownloadTimer()
        {
            System.Timers.ElapsedEventHandler downloadTimerHandler = (sender, e) =>
            {
                DownloadData();
            };
            System.Timers.ElapsedEventHandler downloadStatusTimerHandler = (sender, e) =>
            {
                if (!isDownloading && downloadStatusWait > 0)
                {
                    tb_downloadStatus.SetTextSafe($"Downloading will start in {downloadStatusWait} seconds");
                    downloadStatusWait--;
                }
                else
                {
                    tb_downloadStatus.SetTextSafe(downloadStatusText);
                }
            };

            if (tb_downlaodInterval.Value > 0)
            {
                if (downloadTimer != null && downloadTimer.Enabled)
                {
                    downloadTimer.Elapsed -= downloadTimerHandler;
                    downloadTimer.Stop();
                    downloadTimer.Dispose();
                    downloadTimer.Enabled = false;
                }

                downloadTimer = new System.Timers.Timer((double)tb_downlaodInterval.Value * 1000);
                downloadTimer.Elapsed += downloadTimerHandler;
                downloadTimer.AutoReset = true;
                downloadTimer.Enabled = true;

                downloadStatusWait = tb_downlaodInterval.Value;
            }
            else
            {
                if (downloadTimer != null && downloadTimer.Enabled)
                {
                    downloadTimer.Elapsed -= downloadTimerHandler;
                    downloadTimer.Stop();
                    downloadTimer.Enabled = false;
                }

                downloadTimer?.Dispose();
                downloadStatusWait = 0;
            }

            if (downloadStatusTimer == null)
            {
                downloadStatusTimer = new System.Timers.Timer(1000);
                downloadStatusTimer.Elapsed += downloadStatusTimerHandler;
                downloadStatusTimer.AutoReset = true;
                downloadStatusTimer.Enabled = true;
            }
        }

        async void DownloadData()
        {
            try
            {
                if (isDownloading) return;

                downloadStatusText = "Downloading started...";
                isDownloading = true;

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{StaticInfo.AppSetting?.ApiUrl}/api/PageDataCollector/GetPages");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(jsonString)) return;

                var root = JObject.Parse(jsonString)?.ToObject<GetPagesModel>();
                if (root == null) return;

                var list = root.Data.Where(x => x.Url.StartsWith("https://www.instagram.com") && !x.IsDeleted).ToList();

                downloadStatusText = "Downloading completed";
            }
            catch (Exception)
            {

            }
            finally
            {
                isDownloading = false;
                tb_downlaodInterval.SafeInvoke(() => downloadStatusWait = tb_downlaodInterval.Value);
            }
        }

        void StartBrowser(DS_UserAccount userAccount)
        {
            if (dsBrowsers.TryGetValue(userAccount.Username, out FormDsBrowser value))
            {
                value.Activate();
                return;
            }

            userAccount.PropertyChanged += (s, e) => gv_browsers.SafeInvoke(() => gv_browsers.Refresh());

            var dsForm = new FormDsBrowser(userAccount);
            dsForm.FormClosing += OnFormClosing;
            dsForm.Show();
            dsForm.Activate();

            dsBrowsers.Add(userAccount.Username, dsForm);
            gv_browsers.Refresh();

            void OnFormClosing(object sender, FormClosingEventArgs e)
            {
                if (!dsForm.forceClose)
                {
                    var ans = MessageBox.Show("Do you want to close this browser?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ans == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                dsForm.FormClosing -= OnFormClosing;
                _ = StopBrowser(dsForm);
            }
        }

        async Task StopBrowser(FormDsBrowser dsBrowser, bool forceStop = false)
        {
            dsBrowser.uc_controller.cancellationToken?.Cancel();
            while (dsBrowser.userAccount.IsTaskRunning == true) await Task.Delay(1000);
            dsBrowser.userAccount.IsRunning = false;
            dsBrowser.Close();
            dsBrowsers.Remove(dsBrowser.userAccount.Username);
            gv_browsers.Refresh();
        }

        void LoadAccountsGrid()
        {
            var data = DbHelper.GetAll<DS_UserAccount>();

            userAccounts.Clear();
            foreach (DS_UserAccount browser in data)
            {
                userAccounts.Add(browser);
            }

            gv_browsers.DataSource = userAccounts.OrderByDescending(x => x.IsActive).ToList();
            gv_browsers.Refresh();
        }

        void ShowAccountForm(DS_UserAccount model = null)
        {
            var isNew = model == null;
            if (isNew)
            {
                model = new DS_UserAccount
                {
                    ID = ObjectId.NewObjectId(),
                    UserAgent = StaticInfo.DefaultUserAgent,
                };
            }

            var form = new PropertyForm(isNew ? "Add New Account" : "Edit Account", model);
            var res = form.ShowDialog();
            if (res != DialogResult.OK) return;

            var dbModel = DbHelper.SaveOne(model, x => x.ID == model.ID);
            if (dbModel == null) return;
            if (isNew) userAccounts.Add(dbModel);
            LoadAccountsGrid();
        }

        List<DS_UserAccount> GetSelectedAccounts()
        {
            var list = new List<DS_UserAccount>();
            if (gv_browsers.SelectedRows.Count == 0) return list;

            foreach (DataGridViewRow item in gv_browsers.SelectedRows)
                list.Add(item.DataBoundItem as DS_UserAccount);

            return list.Where(x => x != null).ToList();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            ShowAccountForm();
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            var items = GetSelectedAccounts();
            if (items.Count == 0) return;
            items.ForEach(x => ShowAccountForm(x));
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            var accounts = GetSelectedAccounts();
            if (accounts.Count == 0) return;

            var ans = MessageBox.Show($"Do you want to delete {accounts.Count} records?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            accounts.ForEach(account =>
            {
                if (dsBrowsers.TryGetValue(account.Username, out var dsBrowser))
                {
                    _ = StopBrowser(dsBrowser);
                }

                if (DbHelper.Delete<DS_UserAccount>(account.ID))
                {
                    userAccounts.Remove(account);
                }
            });

            LoadAccountsGrid();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            var items = GetSelectedAccounts();
            if (items == null) return;
            items.ForEach(item => StartBrowser(item));
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            var items = GetSelectedAccounts();
            if (items.Count == 0) return;

            var ans = MessageBox.Show($"Do you want to close {items.Count} browsers?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            items.ForEach(x =>
            {
                if (dsBrowsers.TryGetValue(x.Username, out FormDsBrowser dsBrowser))
                {
                    dsBrowser.forceClose = true;
                    _ = StopBrowser(dsBrowser);
                }
            });
        }

        private void gv_browsers_DoubleClick(object sender, EventArgs e)
        {
            var items = GetSelectedAccounts();
            if (items.Count == 0) return;
            ShowAccountForm(items.First());
        }
        #endregion

        #region Tasks Tab
        private void LoadTasksGrid(List<string> statusFilters = null, List<string> queryFilters = null, List<string> accountFilters = null)
        {
            browserTasks.Clear();
            var data = DbHelper.GetAll<DS_BrowserTask>().ToList();

            if (statusFilters != null && statusFilters.Count > 0)
            {
                data = [.. data.Where(x => statusFilters.Contains(x.Status))];
            }

            if (queryFilters != null && queryFilters.Count > 0)
            {
                data = [.. data.Where(x => queryFilters.Contains(x.QueryAction))];
            }

            if (accountFilters != null && accountFilters.Count > 0)
            {
                data = [.. data.Where(x => accountFilters.Contains(x.DoneBy))];
            }

            foreach (DS_BrowserTask task in data)
            {
                browserTasks.Add(task);
            }

            gv_tasks.DataSource = browserTasks;
            gv_tasks.Refresh();

            gv_tasks.Columns[nameof(DS_BrowserTask.QueryData)].Visible = false;
        }

        private List<DS_BrowserTask> GetSelectedTasks()
        {
            var list = new List<DS_BrowserTask>();
            if (gv_tasks.SelectedRows.Count == 0) return list;

            foreach (DataGridViewRow item in gv_tasks.SelectedRows)
                list.Add(item.DataBoundItem as DS_BrowserTask);

            return list.Where(x => x != null).ToList();
        }

        private async void StartBulkTask(List<DS_BrowserTask> taskList)
        {
            if (taskList.Count == 0)
            {
                MessageBox.Show("Task list is emply.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dsBrowsers.Count == 0)
            {
                MessageBox.Show("Browsers are not running. Start at least one browser", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ans = MessageBox.Show($"Do you want to start {taskList.Count} tasks?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            btn_taskDelete.Enabled = false;
            btn_taskStart.Enabled = false;
            btn_startAll.Enabled = false;

            if (bulkTaskCancelToken != null)
            {
                bulkTaskCancelToken?.Dispose();
                bulkTaskCancelToken = null;
            }

            bulkTaskCancelToken = new CancellationTokenSource();
            var subLists = taskList.SplitInto(dsBrowsers.Count, true);
            var zipLists = dsBrowsers.Zip(subLists, (dsBrowser, tasks) => new { dsBrowser, tasks }).ToList();
            var threads = new List<Task>();

            foreach (var zip in zipLists)
            {
                threads.Add(zip.dsBrowser.Value.uc_controller.StartTasks(zip.tasks, bulkTaskCancelToken));
            }

            await Task.WhenAll(threads);

            btn_taskDelete.Enabled = true;
            btn_taskStart.Enabled = true;
            btn_startAll.Enabled = true;

            LoadTasksGrid();
        }

        private void btn_taskAdd_Click(object sender, EventArgs e)
        {
            var form = new TaskForm();
            var res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                browserTasks.Add(form.GetSelectedTask());
                gv_tasks.Refresh();
            }
        }

        private void btn_taskEdit_Click(object sender, EventArgs e)
        {
            var rows = GetSelectedTasks();
            if (rows.Count == 0) return;

            rows.ForEach(row =>
            {
                var form = new TaskForm(row);
                var res = form.ShowDialog();
                if (res == DialogResult.OK)
                {
                    gv_tasks.Refresh();
                }
            });
        }

        private void btn_taskDelete_Click(object sender, EventArgs e)
        {
            var rows = GetSelectedTasks();
            if (rows.Count == 0) return;

            var ans = MessageBox.Show($"Do you want to delete {rows.Count} records?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            foreach (var row in rows)
            {
                if (DbHelper.Delete<DS_BrowserTask>(row.ID))
                {
                    browserTasks.Remove(row);
                }
            }

            gv_tasks.Refresh();
        }

        private void btn_taskStart_Click(object sender, EventArgs e)
        {
            var dsTasks = GetSelectedTasks();
            StartBulkTask(dsTasks);
        }

        private void btn_taskStop_Click(object sender, EventArgs e)
        {

        }

        private void gv_tasks_DoubleClick(object sender, EventArgs e)
        {
            var tasks = GetSelectedTasks();
            if (tasks.Count == 0) return;

            var form = new TaskForm(tasks.First());
            var res = form.ShowDialog();
            if (res == DialogResult.OK) gv_tasks.Refresh();
        }

        private void btn_taskReload_Click(object sender, EventArgs e)
        {
            LoadTasksGrid();
        }

        private void btn_startAll_Click(object sender, EventArgs e)
        {
            var dsTasks = browserTasks
                .Where(x => x.Status == DS_BrowserTask_Status.Pending)
                .ToList();

            StartBulkTask(dsTasks);
        }

        private void btn_stopAll_Click(object sender, EventArgs e)
        {
            var ans = MessageBox.Show("Do you want to cancel the task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            btn_stopAll.Enabled = false;

            foreach (var item in dsBrowsers)
            {
                _ = item.Value.uc_controller.StopTasks();
            }

            btn_taskDelete.Enabled = true;
            btn_taskStart.Enabled = true;
            btn_startAll.Enabled = true;
            btn_stopAll.Enabled = true;
        }

        private void btn_taskSearch_Click(object sender, EventArgs e)
        {
            var status = new List<string>();
            foreach (var item in filter_status.CheckedItems)
            {
                status.Add(item.ToString());
            }

            var query = new List<string>();
            foreach (var item in filter_query.CheckedItems)
            {
                query.Add(item.ToString());
            }

            var account = new List<string>();
            foreach (var item in filter_account.CheckedItems)
            {
                account.Add(item.ToString());
            }

            LoadTasksGrid(status, query, account);
        }
        #endregion

        #region Setting Tab
        void LoadAppSetting()
        {
            StaticInfo.AppSetting = DbHelper.GetAll<AppSetting>().FirstOrDefault() ?? new AppSetting();

            tb_ipAddress.Text = StaticInfo.AppSetting?.ApiUrl ?? string.Empty;
            tb_downlaodInterval.Value = StaticInfo.AppSetting?.DownloadInterval ?? 0;
            tb_instagrapiSession.Text = StaticInfo.AppSetting?.InstagrapiSessionId ?? string.Empty;
            tb_instagrapiApiUrl.Text = StaticInfo.AppSetting?.InstagrapiUrl ?? string.Empty;
            tb_pyDirectory.Text = StaticInfo.AppSetting.PythonScriptDirectory;
            tb_pyExeDirectory.Text = StaticInfo.AppSetting.PythonExeDirectory;
            tb_pyFileName.Text = StaticInfo.AppSetting.PythonScriptFileName;

            StartDownloadTimer();
        }

        private void btn_saveSettings_Click(object sender, EventArgs e)
        {
            StaticInfo.AppSetting.ApiUrl = tb_ipAddress.Text.Trim();
            StaticInfo.AppSetting.InstagrapiSessionId = tb_instagrapiSession.Text.Trim();
            StaticInfo.AppSetting.InstagrapiUrl = tb_instagrapiApiUrl.Text.Trim();
            StaticInfo.AppSetting.PythonScriptDirectory = tb_pyDirectory.Text.Trim();
            StaticInfo.AppSetting.PythonExeDirectory = tb_pyExeDirectory.Text.Trim();
            StaticInfo.AppSetting.PythonScriptFileName = tb_pyFileName.Text.Trim();

            if (StaticInfo.AppSetting.DownloadInterval != tb_downlaodInterval.Value)
            {
                StaticInfo.AppSetting.DownloadInterval = tb_downlaodInterval.Value;
                StartDownloadTimer();
            }

            DbHelper.SaveOne(StaticInfo.AppSetting, x => x.ID == StaticInfo.AppSetting.ID);
            MessageBox.Show("Settings saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
