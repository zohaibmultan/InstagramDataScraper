#nullable disable

using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Models
{
    public class DS_Browser
    {
        [BsonId]
        [Browsable(false)]
        public ObjectId ID { get; set; }

        [Required]
        [Display(Name = "Username")]
        [Description("Instagram username")]
        public string Username { get; set; }

        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Display(Name = "Email Password")]
        public string EmailPassword { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive {  get; set; }

        [Display(Name = "Is Running")]
        public bool IsRunning {  get; set; }

        [Required]
        [Display(Name = "User Agent")]
        [Description("User agent for browser, must use IPhone Mobile Useragent")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string UserAgent { get; set; }
    }

    public class DS_BrowserLog
    {
        public long ID { get; set; }
        public string Text { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DS_BrowserLogType
    {
        public static string Info = "Info";
        public static string Error = "Error";
        public static string Exception = "Exception";
    }

    public class DS_BrowserTask
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string QueryAction { get; set; }

        public object QueryData { get; set; }

        public string Type { get; set; }

        public bool IsDone{ get; set; }

        public DateTime? CreatedAt{ get; set; }

        public DateTime? DoneAt{ get; set; }

        public string DoneBy { get; set; }
    }
}
