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

        private void btn_save_Click(object sender, EventArgs e)
        {
            Close();
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
                // show custom error message
                MessageBox.Show(results[0].ErrorMessage, "Validation Error");

                // rollback to old value
                e.ChangedItem.PropertyDescriptor.SetValue(obj, e.OldValue);
                propertyGrid.Refresh();
            }
        }
    }
}
