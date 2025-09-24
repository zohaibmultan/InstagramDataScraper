#nullable disable

using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Models
{
    public class DS_UserAccount
    {
        private bool _isActive = true;
        private bool _isRunning = false;
        private bool _isLogin = false;
        private bool _isTaskRunning = false;


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
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        [Display(Name = "Is Running")]
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged(nameof(IsRunning));
                }
            }
        }

        [Display(Name = "Is Login")]
        public bool IsLogin
        {
            get => _isLogin;
            set
            {
                if (_isLogin != value)
                {
                    _isLogin = value;
                    OnPropertyChanged(nameof(IsLogin));
                }
            }
        }

        [Display(Name = "Is Task Running")]
        public bool IsTaskRunning
        {
            get => _isTaskRunning;
            set
            {
                if (_isTaskRunning != value)
                {
                    _isTaskRunning = value;
                    OnPropertyChanged(nameof(IsTaskRunning));
                }
            }
        }



        [Required]
        [Display(Name = "User Agent")]
        [Description("User agent for browser, must use IPhone Mobile Useragent")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string UserAgent { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        public string Username { get; set; }
        public string QueryAction { get; set; }

        public object QueryData { get; set; }

        public string QueryObjectType { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? DoneAt { get; set; }

        public string DoneBy { get; set; }

        public List<string> Logs { get; set; }
    }

    public static class DS_BrowserTask_Status
    {
        public const string Pending  = "Pending";
        public const string Running  = "Running";
        public const string Done  = "Done";
        public const string Error  = "Error";

        public static List<string> GetAllStatus()
        {
            return typeof(DS_BrowserTask_Status)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => f.GetRawConstantValue().ToString())
            .ToList();
        }
    }
}
