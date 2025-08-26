#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SocialMediaDataScraper.Models
{
    public class QueryProfile
    {
        public string Username { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public string ProfileUrl { get; set; }
    }

    public class QuerySinglePost
    {
        public string PostShortCode { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public string PostUrl { get; set; }
    }

    public class QueryBulkPost
    {
        [DisplayName("User Name")]
        [Description("Instagram account username (example instafood_blog)")]
        public string Username { get; set; }

        [DisplayName("Profile URL")]
        [Description("Instagram profile url (example )")]
        [Url(ErrorMessage = "Enter a valid URL")]
        public string ProfileUrl { get; set; }

        [DisplayName("Post Count")]
        [Description("How many posts to fetch (Set 0 to get all posts)")]
        [Required(ErrorMessage = "Post count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Post count must be positive")]
        public int NumberOfPosts { get; set; } = 12;

        [DisplayName("Minimum Wait")]
        [Description("Minimum wait in rate control limit (In seconds)")]
        [Required(ErrorMessage = "Minimum wait is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum wait must be positive")]
        public int MinWait { get; set; } = 5;

        [DisplayName("Maximum Wait")]
        [Description("Maximum wait in rate control limit (In seconds)")]
        [Required(ErrorMessage = "Maximum wait is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Maximum wait must be positive")]
        public int MaxWait { get; set; } = 60;

        [DisplayName("Loop Break")]
        [Description("Loop break after successfull attempts in rate control limit")]
        [Required(ErrorMessage = "Loop break is required")]
        [Range(0, 10, ErrorMessage = "Loop break must be positive")]
        public int LoopBreak { get; set; } = 3;
    }

    public static class QueryAction
    {
        public const string NoAction = "";
        public const string RecheckLoginStatus = "Recheck Login Status";
        public const string GetUserProfile = "Get User Profile";
        public const string GetSinglePost = "Get Single Post";
        public const string GetPostsByUser = "Get All Posts of User";
        public const string GetPostComments = "Get Comments of Post";

        public static List<string> GetAllQueryActions()
        {
            return typeof(QueryAction)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => f.GetRawConstantValue().ToString())
            .ToList();
        }
    }
}
