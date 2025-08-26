#nullable disable

using System.ComponentModel;
using System.Reflection;

namespace SocialMediaDataScraper.Common
{
    public static class PropertyGridExtensions
    {
        public static void FixLabelWidth(this PropertyGrid propertyGrid)
        {
            if (propertyGrid.SelectedObject == null) return;

            var props = TypeDescriptor.GetProperties(propertyGrid.SelectedObject);

            // find the longest display name
            var longestText = props.Cast<PropertyDescriptor>()
                                   .Where(p => p.IsBrowsable)
                                   .OrderByDescending(p => p.DisplayName.Length)
                                   .FirstOrDefault()?.DisplayName ?? "";

            int width = TextRenderer.MeasureText(longestText, propertyGrid.Font).Width + 25;

            // access internal PropertyGridView
            var gridView = propertyGrid.GetType()
                .GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(propertyGrid);

            if (gridView != null)
            {
                PropertyInfo pi = gridView.GetType().GetProperty("LabelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
                if (pi != null)
                {
                    pi.SetValue(gridView, width);
                }
            }
        }
    }
}
