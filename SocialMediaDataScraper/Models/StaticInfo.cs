#nullable disable

using LiteDB;
using SocialMediaDataScraper.Common;
using System.Text.RegularExpressions;

namespace SocialMediaDataScraper.Models
{
    public static class StaticInfo
    {
        public static string DefaultUserAgent { get; set; } = "Mozilla/5.0 (iPhone; CPU iPhone OS 18_6_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1";
        public static string UserSessionDirectory
        {
            get
            {
                var userDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSessions");

                if (!System.IO.File.Exists(userDataFolder))
                {
                    Directory.CreateDirectory(userDataFolder);
                }

                return userDataFolder;
            }
        }

        public static string NormalizeUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string url = input.Trim();

            // Check protocol
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }

            // Insert www if missing (after protocol)
            Uri uri = new Uri(url);
            string host = uri.Host;

            if (!host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                host = "www." + host;
            }

            // Rebuild URL with correct host
            UriBuilder builder = new UriBuilder(uri)
            {
                Host = host
            };

            return builder.ToString();
        }

        public static AppSetting AppSetting { get; set; }

        public static (string Username, string ShortCode) ExtractInstagramInfo(string url)
        {
            // Pattern for URLs with username and short code (reel, post, or feed)
            string patternWithShortCode = @"https:\/\/www\.instagram\.com\/([^\/]+)\/(?:reel|p|feed)\/([^\/]+)\/?";
            // Pattern for URLs with only username
            string patternUsernameOnly = @"https:\/\/www\.instagram\.com\/([^\/]+)\/?";

            // First, try to match URLs with short code
            var matchWithShortCode = Regex.Match(url, patternWithShortCode);
            if (matchWithShortCode.Success)
            {
                return (matchWithShortCode.Groups[1].Value, matchWithShortCode.Groups[2].Value);
            }

            // If no short code, try to match username-only URLs
            var matchUsernameOnly = Regex.Match(url, patternUsernameOnly);
            if (matchUsernameOnly.Success)
            {
                return (matchUsernameOnly.Groups[1].Value, null);
            }

            // Return empty results if URL doesn't match expected patterns
            return (null, null);
        }

        public static void CreateTasksFromUrl(string url)
        {
            var taskCount = 0;
            var (username, shortcode) = ExtractInstagramInfo(url);

            if (!string.IsNullOrEmpty(username))
            {
                var exist1 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetUserProfile && (x.QueryData as QueryProfile).Username == username);
                if (exist1 == null)
                {
                    var task = new DS_BrowserTask()
                    {
                        QueryAction = QueryAction.GetUserProfile,
                        QueryData = new QueryProfile()
                        {
                            Username = username,
                        },
                        QueryObjectType = new QueryProfile().GetType().Name,
                        CreatedAt = DateTime.Now,
                    };
                    if (DbHelper.SaveOne(task) != null) taskCount++;
                }

                var exist2 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetPostsByUser && (x.QueryData as QueryBulkPosts).Username == username);
                if (exist2 == null)
                {
                    var task = new DS_BrowserTask()
                    {
                        QueryAction = QueryAction.GetPostsByUser,
                        QueryData = new QueryBulkPosts()
                        {
                            Username = username,
                            MinWait = 5,
                            MaxWait = 30,
                            LoopBreak = 5,
                            RecordsCount = 500,
                        },
                        QueryObjectType = new QueryBulkPosts().GetType().Name,
                        CreatedAt = DateTime.Now,
                    };
                    if (DbHelper.SaveOne(task) != null) taskCount++;
                }

                var exist3 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetFollowingsAjax && (x.QueryData as QueryFollowingAjax).Username == username);
                if (exist3 == null)
                {
                    var task = new DS_BrowserTask()
                    {
                        QueryAction = QueryAction.GetFollowingsAjax,
                        QueryData = new QueryFollowingAjax()
                        {
                            Username = username,
                            MinWait = 5,
                            MaxWait = 30,
                            LoopBreak = 5,
                            RecordsCount = 500,
                        },
                        QueryObjectType = new QueryFollowingAjax().GetType().Name,
                        CreatedAt = DateTime.Now,
                    };
                    if (DbHelper.SaveOne(task) != null) taskCount++;
                }
            }

            if (!string.IsNullOrEmpty(shortcode))
            {
                var exist1 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetSinglePost && (x.QueryData as QuerySinglePost).PostShortCode == shortcode);
                if (exist1 == null)
                {
                    var task = new DS_BrowserTask()
                    {
                        QueryAction = QueryAction.GetSinglePost,
                        QueryData = new QuerySinglePost()
                        {
                            PostShortCode = shortcode,
                        },
                        QueryObjectType = new QuerySinglePost().GetType().Name,
                        CreatedAt = DateTime.Now,
                    };
                    if (DbHelper.SaveOne(task) != null) taskCount++;
                }
            }

            var notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Information,
                Visible = true,
                BalloonTipTitle = "Success",
                BalloonTipText = $"{taskCount} tasks created",
                BalloonTipIcon = ToolTipIcon.Info,
            };
            notifyIcon.ShowBalloonTip(1000);
        }

        public static void CreateTasksFromUserId(string userPk, string username)
        {
            var taskCount = 0;

            /*try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{AppSetting.InstagrapiUrl}/user/username_from_id");
                request.Headers.Add("Accept", "application/json");

                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("sessionid", AppSetting.InstagrapiSessionId));
                collection.Add(new("user_id", userPk));

                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response != null)
                {
                    username = await response.Content.ReadAsStringAsync();
                    username = username.Replace("\"", "");
                }
            }
            catch (Exception)
            {}*/

            var exist1 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetUserProfile && ((x.QueryData as QueryProfile).Username == username || (x.QueryData as QueryProfile).UserPk == userPk));
            if (exist1 == null)
            {
                var task = new DS_BrowserTask()
                {
                    QueryAction = QueryAction.GetUserProfile,
                    QueryData = new QueryProfile()
                    {
                        Username = username,
                        UserPk = userPk
                    },
                    QueryObjectType = new QueryProfile().GetType().Name,
                    CreatedAt = DateTime.Now,
                };
                if (DbHelper.SaveOne(task) != null) taskCount++;
            }

            var exist2 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetPostsByUser && ((x.QueryData as QueryBulkPosts).Username == username || (x.QueryData as QueryBulkPosts).UserPk == userPk));
            if (exist2 == null)
            {
                var task = new DS_BrowserTask()
                {
                    QueryAction = QueryAction.GetPostsByUser,
                    QueryData = new QueryBulkPosts()
                    {
                        Username = username,
                        UserPk = userPk,
                        MinWait = 5,
                        MaxWait = 30,
                        LoopBreak = 5,
                        RecordsCount = 500,
                    },
                    QueryObjectType = new QueryBulkPosts().GetType().Name,
                    CreatedAt = DateTime.Now,
                };
                if (DbHelper.SaveOne(task) != null) taskCount++;
            }

            var exist3 = DbHelper.GetOne<DS_BrowserTask>(x => x.QueryAction == QueryAction.GetFollowingsAjax && ((x.QueryData as QueryFollowingAjax).Username == username || (x.QueryData as QueryFollowingAjax).UserPK.ToString() == userPk));
            if (exist3 == null)
            {
                var task = new DS_BrowserTask()
                {
                    QueryAction = QueryAction.GetFollowingsAjax,
                    QueryData = new QueryFollowingAjax()
                    {
                        Username = username,
                        UserPK = long.Parse(userPk),
                        MinWait = 5,
                        MaxWait = 30,
                        LoopBreak = 5,
                        RecordsCount = 500,
                    },
                    QueryObjectType = new QueryFollowingAjax().GetType().Name,
                    CreatedAt = DateTime.Now,
                };
                if (DbHelper.SaveOne(task) != null) taskCount++;
            }

            var notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Information,
                Visible = true,
                BalloonTipTitle = "Success",
                BalloonTipText = $"{taskCount} tasks created",
                BalloonTipIcon = ToolTipIcon.Info,
            };
            notifyIcon.ShowBalloonTip(1000);

            return;
        }
    }

    public class AppSetting
    {
        [BsonId]
        public int ID { get; set; }

        public string ApiUrl { get; set; }
        public decimal DownloadInterval { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string InstagrapiUrl { get; set; }
        public string InstagrapiSessionId { get; set; }
        public string PythonScriptDirectory { get; set; }
        public string PythonExeDirectory { get; set; }
        public string PythonScriptFileName { get; set; }
    }
}
