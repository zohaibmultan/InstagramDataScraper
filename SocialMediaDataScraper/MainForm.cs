#nullable disable

using LiteDB;
using Newtonsoft.Json.Linq;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;

namespace SocialMediaDataScraper
{
    public partial class MainForm : Form
    {
        BindingList<DS_Browser> accounts = [];
        BindingList<DS_BrowserTask> tasks = [];
        Dictionary<string, FormDsBrowser> forms = [];

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
        }

        void LoadAppSetting()
        {
            StaticInfo.AppSetting = DbHelper.GetAll<AppSetting>().FirstOrDefault() ?? new AppSetting();

            tb_ipAddress.Text = StaticInfo.AppSetting?.ApiUrl ?? string.Empty;
            tb_downlaodInterval.Value = StaticInfo.AppSetting?.DownloadInterval ?? 0;
            tb_instagrapiSession.Text = StaticInfo.AppSetting?.InstagrapiSessionId ?? string.Empty;
            tb_instagrapiApiUrl.Text = StaticInfo.AppSetting?.InstagrapiUrl ?? string.Empty;

            StartDownloadTimer();
        }

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

        void LoadAccountsGrid()
        {
            gv_browsers.DataSource = accounts;
            var data = DbHelper.GetAll<DS_Browser>();
            accounts.Clear();

            foreach (DS_Browser browser in data)
            {
                accounts.Add(browser);
            }

            gv_browsers.Refresh();
        }

        void LoadTasksGrid()
        {
            tasks.Clear();
            var data = DbHelper.GetAll<DS_BrowserTask>().OrderByDescending(x => x.CreatedAt);

            foreach (DS_BrowserTask task in data)
            {
                tasks.Add(task);
            }

            gv_tasks.DataSource = tasks;
            gv_tasks.Refresh();

            gv_tasks.Columns[nameof(DS_BrowserTask.QueryData)].Visible = false;
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

        List<DS_Browser> GetSelectedAccounts()
        {
            var list = new List<DS_Browser>();
            if (gv_browsers.SelectedRows.Count == 0) return list;

            foreach (DataGridViewRow item in gv_browsers.SelectedRows)
                list.Add(item.DataBoundItem as DS_Browser);

            return list.Where(x => x != null).ToList();
        }

        List<DS_BrowserTask> GetSelectedTasks()
        {
            var list = new List<DS_BrowserTask>();
            if (gv_tasks.SelectedRows.Count == 0) return list;

            foreach (DataGridViewRow item in gv_tasks.SelectedRows)
                list.Add(item.DataBoundItem as DS_BrowserTask);

            return list.Where(x => x != null).ToList();
        }

        void ShowAccountForm(DS_Browser model = null)
        {
            var isNew = model == null;
            if (isNew)
            {
                model = new DS_Browser
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
            if (isNew) accounts.Add(dbModel);
            gv_browsers.Refresh();
        }



        void StartBrowser(DS_Browser browser)
        {
            if (forms.ContainsKey(browser.Username))
            {
                forms[browser.Username].Activate();
                return;
            }

            browser.PropertyChanged += (s, e) =>gv_browsers.SafeInvoke(() => gv_browsers.Refresh());

            var dsForm = new FormDsBrowser(browser);
            dsForm.FormClosing += (s, e) => forms.Remove(browser.Username);


            dsForm.Show();
            dsForm.Activate();

            forms.Add(browser.Username, dsForm);
            gv_browsers.Refresh();
        }

        void StopBrowser(DS_Browser browser)
        {
            if (!forms.ContainsKey(browser.Username)) return;
            if (browser.IsRunning)
            {
                var ans = MessageBox.Show("Browser task is running. Do you want to close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans != DialogResult.Yes) return;
            }

            forms[browser.Username].Close();
            forms.Remove(browser.Username);

            browser.IsRunning = false;
            gv_browsers.Refresh();
        }



        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadAccountsGrid();
            LoadTasksGrid();
            LoadAppSetting();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var ans = MessageBox.Show("Do you want to close?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) e.Cancel = true;

            downloadStatusTimer?.Dispose();

            if (downloadTimer != null)
            {
                downloadTimer.Stop();
                downloadTimer.Dispose();
            }

            foreach (var form in forms)
            {
                form.Value.CancelRunningTask();
            }
        }


        #region Events Account Tab
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
            var items = GetSelectedAccounts();
            if (items.Count == 0) return;

            var ans = MessageBox.Show($"Do you want to delete {items.Count} records?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            items.ForEach(item =>
            {
                if (DbHelper.Delete<DS_Browser>(item.ID))
                {
                    accounts.Remove(item);
                }
            });

            gv_browsers.Refresh();
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

            items.ForEach(x =>
            {
                if (forms.ContainsKey(x.Username))
                {
                    StopBrowser(x);
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

        #region Events Tasks Tab
        private void btn_taskAdd_Click(object sender, EventArgs e)
        {
            var form = new TaskForm();
            var res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                tasks.Add(form.GetSelectedTask());
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
                    tasks.Remove(row);
                }
            }

            gv_tasks.Refresh();
        }

        private void btn_taskStart_Click(object sender, EventArgs e)
        {

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
            if(forms.Count == 0) return;
            
            var ans = MessageBox.Show("Do you want to start all tasks?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            var mainList = DbHelper.GetAll<DS_BrowserTask>().Where(x => !x.IsDone).ToList();
            var subLists = mainList.SplitInto(forms.Count, true);

            forms.Zip(subLists, (form, tasks) => new { form, tasks }).ToList().ForEach(x => x.form.Value.SetTaskList(x.tasks));
        }

        private void btn_stopAll_Click(object sender, EventArgs e)
        {
            foreach (var form in forms)
            {
                form.Value.CancelRunningTask();
            }
        }
        #endregion

        #region Events Setting Tab
        private void btn_saveSettings_Click(object sender, EventArgs e)
        {
            StaticInfo.AppSetting.ApiUrl = tb_ipAddress.Text.Trim();
            StaticInfo.AppSetting.InstagrapiSessionId = tb_instagrapiSession.Text.Trim();
            StaticInfo.AppSetting.InstagrapiUrl = tb_instagrapiApiUrl.Text.Trim();

            if (StaticInfo.AppSetting.DownloadInterval != tb_downlaodInterval.Value)
            {
                StaticInfo.AppSetting.DownloadInterval = tb_downlaodInterval.Value;
                StartDownloadTimer();
            }

            DbHelper.SaveOne(StaticInfo.AppSetting, x => x.ID == StaticInfo.AppSetting.ID);
        }
        #endregion
    }
}
