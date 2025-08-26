using LiteDB;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;

namespace SocialMediaDataScraper
{
    public partial class MainForm : Form
    {
        BindingList<DS_Browser> browsers = new BindingList<DS_Browser>();

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            var model = new DS_Browser
            {
                ID = ObjectId.NewObjectId(),
                UserAgent = "Mozilla/5.0",
            };

            if (new PropertyForm("Add New Browser", model).ShowDialog() != DialogResult.OK) return;

            if (!DbHelper.Save<DS_Browser>(model)) return;

            browsers.Add(model);
            gv_browsers.Refresh();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (gv_browsers.SelectedRows.Count < 1) return;
            var model = gv_browsers.SelectedRows[0].DataBoundItem as DS_Browser;
            if (model == null) return;
            if (model.IsRunning) return;

            if (!DbHelper.Delete<DS_Browser>(model.ID)) return;

            browsers.Remove(model);
            gv_browsers.Refresh();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (gv_browsers.SelectedRows.Count < 1) return;
            var model = gv_browsers.SelectedRows[0].DataBoundItem as DS_Browser;
            if (model == null) return;
            if (model.IsRunning)
            {
                MessageBox.Show("Already running");
                return;
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {

        }
    }
}
