#nullable disable

namespace SocialMediaDataScraper.Common
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Safely executes an action on the control. If invoke is required, it uses Invoke.
        /// </summary>
        /// <param name="control">The control to act on.</param>
        /// <param name="action">The action to perform.</param>
        public static void SafeInvoke(this Control control, Action action)
        {
            try
            {
                if (control == null || control.IsDisposed) return;

                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Safely sets the text of a control.
        /// </summary>
        /// <param name="control">The control to update.</param>
        /// <param name="text">The text to set.</param>
        public static void SetTextSafe(this Control control, string text)
        {
            control.SafeInvoke(() => control.Text = text);
        }
    }

    public static class ListExtensions
    {
        public static List<List<T>> SplitInto<T>(this List<T> source, int n, bool shuffle = false)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (n <= 0) throw new ArgumentException("Number of sublists must be greater than 0.", nameof(n));

            var list = source;
            if (shuffle)
            {
                list = list.OrderBy(_ => Random.Shared.Next()).ToList();
            }

            int chunkSize = (int)Math.Ceiling((double)list.Count / n);
            return list.Chunk(chunkSize).Select(chunk => chunk.ToList()).ToList();
        }
    }
}
