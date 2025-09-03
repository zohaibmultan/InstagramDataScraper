#nullable disable

using System.ComponentModel.DataAnnotations;

namespace SocialMediaDataScraper
{
    public partial class PropertyForm : Form
    {
        public PropertyForm()
        {
            InitializeComponent();
        }

        public PropertyForm(string title, object model)
        {
            InitializeComponent();
            Text = title;
            propertyGrid.SelectedObject = model;
        }

        public void SetObject(object model)
        {
            propertyGrid.SelectedObject = model;
            propertyGrid.Refresh();
        }

        public object GetObject()
        {
            return propertyGrid.SelectedObject;
        }

        public static (bool, IEnumerable<string>) ValidateObject(object obj)
        {
            if (obj == null) return (false, ["Object is null"]);

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

        private void PropertyForm_Shown(object sender, EventArgs e)
        {
            propertyGrid.Refresh();
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

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
