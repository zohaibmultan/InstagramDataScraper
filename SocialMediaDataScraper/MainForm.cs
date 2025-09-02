#nullable disable

using LiteDB;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;

namespace SocialMediaDataScraper
{
    public partial class MainForm : Form
    {
        BindingList<DS_Browser> browsers = [];
        Dictionary<string, FormDsBrowser> forms = [];

        public MainForm()
        {
            InitializeComponent();

            gv_browsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gv_browsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv_browsers.MultiSelect = false;
            gv_browsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gv_browsers.ColumnHeadersHeight = 40;
            gv_browsers.RowTemplate.Height = 40;
            gv_browsers.ReadOnly = true;
            gv_browsers.AllowUserToAddRows = false;
            gv_browsers.AllowUserToDeleteRows = false;
            gv_browsers.AllowUserToResizeRows = false;
            gv_browsers.RowHeadersVisible = false;
        }

        void LoadGrid()
        {
            gv_browsers.DataSource = browsers;
            var data = DbHelper.GetAll<DS_Browser>();
            browsers.Clear();

            foreach (DS_Browser browser in data)
            {
                browsers.Add(browser);
            }

            gv_browsers.Refresh();
        }

        DS_Browser GetSelectedRow()
        {
            if (gv_browsers.SelectedRows.Count == 0) return null;
            return gv_browsers.SelectedRows[0].DataBoundItem as DS_Browser;
        }

        void ShowForm(DS_Browser model = null)
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
            if (isNew) browsers.Add(dbModel);
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            var item = GetSelectedRow();
            if (item == null || item.IsRunning) return;

            var ans = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            if (!DbHelper.Delete<DS_Browser>(item.ID)) return;

            browsers.Remove(item);
            gv_browsers.Refresh();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            var item = GetSelectedRow();
            if (item == null) return;
            StartBrowser(item);
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            var item = GetSelectedRow();
            if (item == null || !forms.ContainsKey(item.Username)) return;
            StopBrowser(item);
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            var item = GetSelectedRow();
            if (item == null || item.IsRunning) return;
            ShowForm(item);
        }

        private void gv_browsers_DoubleClick(object sender, EventArgs e)
        {
            var item = GetSelectedRow();
            if (item == null || item.IsRunning) return;
            ShowForm(item);
        }

        private void btn_startAll_Click(object sender, EventArgs e)
        {
            foreach (var item in browsers)
            {
                if (item.IsActive)
                    StartBrowser(item);
            }
        }

        private void btn_stopAll_Click(object sender, EventArgs e)
        {
            foreach (var item in browsers)
            {
                StopBrowser(item);
            }
        }
    }
}
