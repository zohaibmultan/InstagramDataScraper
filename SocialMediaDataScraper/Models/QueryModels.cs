#nullable disable

using SocialMediaDataScraper.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SocialMediaDataScraper.Models
{
    public abstract class QueryBulk
    {
        [DisplayName("Number of Records")]
        [Description("How many records to fetch (Set 0 to get all)")]
        [Required(ErrorMessage = "Record count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Post count must be positive")]
        public int RecordsCount { get; set; } = 12;

        [DisplayName("Minimum Wait")]
        [Description("Minimum wait in rate control limit (In seconds)")]
        [Required(ErrorMessage = "Minimum wait is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum wait must be positive")]
        public int MinWait { get; set; } = 5;

        [DisplayName("Maximum Wait")]
        [Description("Maximum wait in rate control limit (In seconds)")]
        [Required(ErrorMessage = "Maximum wait is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Maximum wait must be positive")]
        public int MaxWait { get; set; } = 30;

        [DisplayName("Loop Break")]
        [Description("Loop break after successfull attempts in rate control limit")]
        [Required(ErrorMessage = "Loop break is required")]
        [Range(0, 10, ErrorMessage = "Loop break must be positive")]
        public int LoopBreak { get; set; } = 5;
    }

    public class QueryProfile
    {
        [DisplayName("Username")]
        [Description("Instagram username (example insta_bloger)")]
        [RequireAtLeastOne(nameof(ProfileUrl), ErrorMessage = "At least username or profile url must be provided.")]
        public string Username { get; set; }

        [DisplayName("Profile Url")]
        [Description("Instagram profile URL")]
        [RequireAtLeastOne(nameof(Username), ErrorMessage = "At least username or profile url must be provided.")]
        [Url(ErrorMessage = "Invalid URL")]
        public string ProfileUrl { get; set; }

        [Browsable(false)]
        [DisplayName("Profile User ID")]
        [Description("Instagram profile user id")]
        public string UserPk { get; set; }
    }

    public class QuerySinglePost
    {
        [DisplayName("Post Shortcode")]
        [Description("Instagram post shortcode (example Cx34gb7l43)")]
        [RequireAtLeastOne(nameof(PostUrl), ErrorMessage = "At least one of Post Shortcode or Post URL must be provided.")]
        public string PostShortCode { get; set; }

        [DisplayName("Post URL")]
        [Description("Instagram post URL")]
        [Url(ErrorMessage = "Invalid URL")]
        [RequireAtLeastOne(nameof(PostShortCode), ErrorMessage = "At least one of Post Shortcode or Post URL must be provided.")]
        public string PostUrl { get; set; }
    }

    public class QueryBulkPosts : QueryBulk
    {
        [DisplayName("User Name")]
        [Description("Instagram account username (example instafood_blog)")]
        [RequireAtLeastOne(nameof(ProfileUrl), ErrorMessage = "At least username or profile url must be provided.")]
        public string Username { get; set; }

        [DisplayName("Profile URL")]
        [Description("Instagram profile URL")]
        [Url(ErrorMessage = "Enter a valid URL")]
        [RequireAtLeastOne(nameof(Username), ErrorMessage = "At least username or profile url must be provided.")]
        public string ProfileUrl { get; set; }

        [Browsable(false)]
        [DisplayName("Profile User ID")]
        [Description("Instagram profile user id")]
        public string UserPk { get; set; }
    }

    public class QueryFollowing : QueryBulk
    {
        [DisplayName("Username")]
        [Description("Instagram username (example insta_bloger)")]
        [RequireAtLeastOne(nameof(ProfileUrl), ErrorMessage = "At least username or profile url must be provided.")]
        public string Username { get; set; }

        [DisplayName("Profile Url")]
        [Description("Instagram profile URL")]
        [RequireAtLeastOne(nameof(Username), ErrorMessage = "At least username or profile url must be provided.")]
        [Url(ErrorMessage = "Invalid URL")]
        public string ProfileUrl { get; set; }
    }

    public class QueryFollowingAjax : QueryBulk
    {
        [DisplayName("User PK")]
        [Description("Instagram user pk id (example 2461356446)")]
        [Required(ErrorMessage = "User pk is required")]
        public long UserPK { get; set; }

        [DisplayName("User Name")]
        [Description("Instagram account username (example instafood_blog)")]
        public string Username { get; set; }
    }

    public class QueryPostComments : QueryBulk
    {
        [DisplayName("Post Shortcode")]
        [Description("Instagram post shortcode (example Cx34gb7l43)")]
        [Required(ErrorMessage = "Post shortcode is required")]
        public string PostShortCode { get; set; }
    }

    public static class QueryAction
    {
        public const string NoAction = "";
        public const string RecheckLoginStatus = "Recheck Login Status";
        public const string GetUserProfile = "Get User Profile";
        public const string GetSinglePost = "Get Single Post";
        public const string GetPostsByUser = "Get All Posts of User";
        public const string GetPostComments = "Get Comments of Post";
        public const string GetFollowings = "Get Followings of User";
        public const string GetFollowingsAjax = "Get Followings of User (Ajax)";
        public const string MonitorFollowRequest = "Monitor Follow Request";
        public const string GetUserPkFromUsername = "Get user PK from username";


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
