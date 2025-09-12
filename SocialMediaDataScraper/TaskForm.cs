#nullable disable

using LiteDB;
using Newtonsoft.Json;
using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialMediaDataScraper
{
    public partial class TaskForm : Form
    {
        private DS_BrowserTask selectedTask { get; set; }

        public TaskForm(DS_BrowserTask task = null)
        {
            InitializeComponent();
            selectedTask = task;
            cb_commands.DataSource = new BindingList<string>(QueryAction.GetAllQueryActions());
            cb_commands.SelectedItem = QueryAction.NoAction;

            cb_status.DataSource = new BindingList<string>(DS_BrowserTask_Status.GetAllStatus());
            cb_commands.SelectedItem = DS_BrowserTask_Status.Pending;

            var accounts = DbHelper.GetAll<DS_UserAccount>().Select(x => x.Username).ToList();
            accounts.Insert(0, "");
            cb_doneBy.DataSource = new BindingList<string>(accounts);
            cb_doneBy.SelectedItem = "";

            LoadUI();
        }

        public DS_BrowserTask GetSelectedTask()
        {
            return selectedTask;
        }

        private void LoadUI()
        {
            Text = selectedTask == null ? "Add New Task" : "Edit Task";
            cb_commands.SelectedItem = selectedTask?.QueryAction ?? QueryAction.NoAction;
            cb_status.SelectedItem = selectedTask?.Status;
            cb_doneBy.SelectedItem = selectedTask?.DoneBy;
            tb_DoneAt.Text = selectedTask?.DoneAt?.ToString("dd-MMM-yyyy hh:mm:ss tt");
            tb_logs.Text = selectedTask?.Logs == null ? "" : string.Join("\n", selectedTask?.Logs);
            propertyGrid.SelectedObject = selectedTask?.QueryData;
            btn_taskResult.Enabled = selectedTask != null;
            propertyGrid.Refresh();
        }

        private (bool, IEnumerable<string>) ValidateObject(object obj)
        {
            if (obj == null) return (false, ["Provide all the required details"]);

            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);

            if (!valid)
            {
                return (false, results.Select(r => r.ErrorMessage).Distinct());
            }

            return (true, null);
        }

        private void ShowTaskResult(object data)
        {
            if (selectedTask == null) return;

            var jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
            if (string.IsNullOrEmpty(jsonStr)) return;

            var textBox = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Text = jsonStr,
                ReadOnly = true,
            };

            var form = new Form();
            form.Controls.Add(textBox);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.8);
            form.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.7);
            form.Text = selectedTask?.ID.ToString();
            form.ShowDialog();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var obj = propertyGrid.SelectedObject;

            var context = new ValidationContext(obj)
            {
                MemberName = e.ChangedItem.PropertyDescriptor.Name
            };

            var results = new List<ValidationResult>();
            bool valid = Validator.TryValidateProperty(e.ChangedItem.Value, context, results);

            if (!valid)
            {
                MessageBox.Show(results[0].ErrorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.ChangedItem.PropertyDescriptor.SetValue(obj, e.OldValue);
                propertyGrid.Refresh();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var (status, errors) = ValidateObject(propertyGrid.SelectedObject);
            if (!status)
            {
                MessageBox.Show(string.Join("\n", errors), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var isNew = selectedTask == null;
            if (isNew)
            {
                selectedTask = new DS_BrowserTask()
                {
                    QueryObjectType = propertyGrid.SelectedObject.GetType().Name,
                    QueryAction = cb_commands.SelectedItem as string,
                    Status = DS_BrowserTask_Status.Pending,
                    QueryData = propertyGrid.SelectedObject,
                    CreatedAt = DateTime.Now,
                };

                DbHelper.SaveOne(selectedTask);
            }
            else
            {
                selectedTask.QueryObjectType = propertyGrid.SelectedObject.GetType().Name;
                selectedTask.QueryAction = cb_commands.SelectedItem as string;
                selectedTask.Status = cb_status.SelectedItem as string;
                selectedTask.DoneBy = cb_doneBy.SelectedItem as string;
                selectedTask.QueryData = propertyGrid.SelectedObject;
                selectedTask.CreatedAt ??= DateTime.Now;
                DbHelper.UpdateOne(selectedTask);
            }

            MessageBox.Show("Task saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUI();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cb_commands_SelectedIndexChanged(object sender, EventArgs e)
        {
            var command = cb_commands.SelectedItem as string;
            if (string.IsNullOrEmpty(command)) return;

            switch (command)
            {
                case QueryAction.RecheckLoginStatus:
                    propertyGrid.SelectedObject = new QueryProfile();
                    break;

                case QueryAction.GetUserProfile:
                    propertyGrid.SelectedObject = new QueryProfile();
                    break;

                case QueryAction.GetSinglePost:
                    propertyGrid.SelectedObject = new QuerySinglePost();
                    break;

                case QueryAction.GetPostsByUser:
                    propertyGrid.SelectedObject = new QueryBulkPosts();
                    break;

                case QueryAction.GetFollowings:
                    propertyGrid.SelectedObject = new QueryFollowing();
                    break;

                case QueryAction.GetFollowingsAjax:
                    propertyGrid.SelectedObject = new QueryFollowingAjax();
                    break;

                case QueryAction.GetPostComments:
                    propertyGrid.SelectedObject = new QueryPostComments();
                    break;
            }

            propertyGrid.Refresh();
        }

        private void btn_taskResult_Click(object sender, EventArgs e)
        {
            if (selectedTask == null)
            {
                return;
            }

            switch (selectedTask.QueryAction)
            {
                case QueryAction.GetUserProfile:
                    var profile = DbHelper.GetOne<InstaProfile>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(profile);
                    break;

                case QueryAction.GetSinglePost:
                    var post = DbHelper.GetOne<InstaPostVr2>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(post);
                    break;

                case QueryAction.GetPostsByUser:
                    var posts = DbHelper.Get<InstaPost>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(posts);
                    break;

                case QueryAction.GetFollowings:
                    var followings = DbHelper.Get<InstaFollowing>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(followings);
                    break;

                case QueryAction.GetFollowingsAjax:
                    var followingAjax = DbHelper.Get<InstaFollowing>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(followingAjax);
                    break;

                case QueryAction.GetPostComments:
                    var comments = DbHelper.Get<InstaComment>(x => x.taskId == selectedTask.ID);
                    ShowTaskResult(comments);
                    break;
            }
        }
    }
}
