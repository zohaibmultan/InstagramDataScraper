#nullable disable

using SocialMediaDataScraper.Common;
using SocialMediaDataScraper.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaDataScraper
{
    public partial class TaskForm : Form
    {
        private DS_BrowserTask selectedTask{ get; set; }

        public TaskForm(DS_BrowserTask task = null)
        {
            InitializeComponent();
            selectedTask = task;
            cb_commands.DataSource = new BindingList<string>(QueryAction.GetAllQueryActions());
            cb_commands.SelectedItem = QueryAction.NoAction;
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
            propertyGrid.SelectedObject = selectedTask?.QueryData;
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

        private void btn_run_Click(object sender, EventArgs e)
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
                    QueryAction = cb_commands.SelectedItem as string,
                    QueryData = propertyGrid.SelectedObject,
                    Type = propertyGrid.SelectedObject.GetType().Name,
                    CreatedAt = DateTime.Now,
                };

                DbHelper.SaveOne(selectedTask);
            }
            else
            {
                selectedTask.QueryAction = cb_commands.SelectedItem as string;
                selectedTask.QueryData = propertyGrid.SelectedObject;
                selectedTask.Type = propertyGrid.SelectedObject.GetType().Name;
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
    }
}
