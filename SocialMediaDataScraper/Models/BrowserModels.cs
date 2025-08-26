#nullable disable

using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Models
{
    public class DS_Browser
    {
        public ObjectId ID { get; set; }
        public string UserAgent { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public bool IsRunning {  get; set; }
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
}
