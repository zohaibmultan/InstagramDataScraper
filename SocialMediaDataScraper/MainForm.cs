#nullable disable

using LiteDB;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace SocialMediaDataScraper
{
    public partial class MainForm : Form
    {
        BindingList<DS_Browser> accounts = [];
        BindingList<DS_BrowserTask> tasks = [];
        Dictionary<string, FormDsBrowser> forms = [];

        public MainForm()
        {
            InitializeComponent();

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

            var dsForm = new FormDsBrowser(browser);
            dsForm.FormClosing += (s, e) =>
            {
                gv_browsers.SafeInvoke(() => gv_browsers.Refresh());
                forms.Remove(browser.Username);
            };

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
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var ans = MessageBox.Show("Do you want to close?", "Confirm!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) e.Cancel = true;

            foreach (var form in forms)
            {
                form.Value.CancelRunningTask();
            }
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

        private void btn_startAll_Click(object sender, EventArgs e)
        {
            foreach (var item in accounts)
            {
                if (item.IsActive)
                    StartBrowser(item);
            }
        }

        private void btn_stopAll_Click(object sender, EventArgs e)
        {
            foreach (var item in accounts)
            {
                StopBrowser(item);
            }
        }



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
    }
}
