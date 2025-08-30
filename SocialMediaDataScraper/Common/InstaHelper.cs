#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SocialMediaDataScraper.Models
{
    public static class InstaHelper
    {
        private static Random random = new Random();

        private static (bool, string) GetScriptFromFile(string scriptFileName)
        {
            var scriptPath = Path.Combine(WebViewHelper.ScriptDirectory, scriptFileName);
            if (!Path.Exists(scriptPath)) return (false, $"{scriptFileName} not found");

            var scriptText = File.ReadAllText(scriptPath);
            if (string.IsNullOrEmpty(scriptText)) return(false, $"{scriptFileName} is empty");

            return (true,  scriptText);
        }

        private static async Task<WebViewJsExecuteResult> ExecuteJsAsync(WebView2 webView, string jsCode, int waitInSeconds = 15)
        {
            var tcs = new TaskCompletionSource<WebViewJsExecuteResult>();

            void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
            {
                try
                {
                    var message = args.WebMessageAsJson;
                    var json = JsonConvert.DeserializeObject<dynamic>(message);
                    tcs.SetResult(new WebViewJsExecuteResult()
                    {
                        Status = json.status,
                        ResultContent = json.body ?? "",
                        Errors = null,
                    });
                }
                catch (Exception ex)
                {
                    tcs.SetResult(new WebViewJsExecuteResult()
                    {
                        Status = null,
                        ResultContent = null,
                        Errors = ex.GetAllInnerMessages(),
                    });
                }
            }

            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync("console.log('--- WebView Script Execuation ---')");

                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
                await webView.CoreWebView2.ExecuteScriptAsync(jsCode);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(waitInSeconds));
                cts.Token.Register(() => tcs.TrySetResult(new WebViewJsExecuteResult()
                {
                    Status = null,
                    ResultContent = null,
                    Errors = new List<string>()
                    {
                        "Request timed out"
                    }
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new WebViewJsExecuteResult()
                {
                    Status = null,
                    ResultContent = null,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                webView.CoreWebView2.WebMessageReceived -= WebView_WebMessageReceived;
                await webView.CoreWebView2.ExecuteScriptAsync("console.log('--- End ---')");
            }
        }

        public static async Task<InstaResult<List<InstaReel>>> TestLogin(WebView2 webView, string username, int waitInSeconds = 60)
        {
            var requestFilter = "graphql/query";
            var requestUrl = $"https://www.instagram.com/{username}";
            var responseFilterKey = "X-Root-Field-Name".ToLower();
            var responseFilterValue = "xdt_api__v1__feed__reels_tray".ToLower();
            var tcs = new TaskCompletionSource<InstaResult<List<InstaReel>>>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.ToLower() == responseFilterKey && x.Value.ToLower() == responseFilterValue);
                return check1 && check2;
            }

            async void WebView_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request) || e.Response == null) return;

                    using (var stream = await e.Response.GetContentAsync())
                    {
                        if (stream == null) return;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            if (reader == null) return;
                            var content = await reader.ReadToEndAsync();
                            if (string.IsNullOrWhiteSpace(content)) return;

                            var root = JObject.Parse(content);
                            if (root == null) return;

                            var feed = root["data"]?[responseFilterValue]?["tray"] as JArray;
                            if (feed == null) return;

                            var reels = feed.ToObject<List<InstaReel>>();

                            tcs.SetResult(new InstaResult<List<InstaReel>>()
                            {
                                Status = true,
                                Content = reels,
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                webView.Source = new Uri(requestUrl);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(waitInSeconds));
                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<List<InstaReel>>()
                {
                    Status = false,
                    Content = null,
                    Errors = new List<string>() { "Request timed out" }
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaReel>>
                {
                    Status = false,
                    Content = null,
                    Errors = ex.GetAllInnerMessages(),
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceResponseReceived -= WebView_WebResourceResponseReceived;
            }
        }


        public static async Task<InstaResult<InstaProfile>> GetProfileByUsername(WebView2 webView, string username, int waitInSeconds = 60)
        {
            var requestUrl = $"https://www.instagram.com/{username}";
            return await GetProfileByUrl(webView, requestUrl, waitInSeconds);
        }

        public static async Task<InstaResult<InstaProfile>> GetProfileByUrl(WebView2 webView, string requestUrl, int waitInSeconds = 60)
        {
            var requestFilter = "graphql/query";
            var tcs = new TaskCompletionSource<InstaResult<InstaProfile>>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals("X-Root-Field-Name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals("fetch__XDTUserDict", StringComparison.CurrentCultureIgnoreCase));
                var check3 = Request.Headers.Any(x => x.Key.Equals("x-fb-friendly-name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals("PolarisProfilePageContentQuery", StringComparison.CurrentCultureIgnoreCase));
                return check1 && check2 && check3;
            }

            async void WebView_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request) || e.Response == null) return;

                    using (var stream = await e.Response.GetContentAsync())
                    {
                        if (stream == null) return;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            if (reader == null) return;
                            var content = await reader.ReadToEndAsync();
                            if (string.IsNullOrWhiteSpace(content)) return;

                            var root = JObject.Parse(content);
                            if (root == null) return;

                            var user = (JObject)root["data"]?["user"];
                            if (user == null) return;

                            var profile = user.ToObject<InstaProfile>();
                            if (profile == null) return;
                            if (!profile.Validate()) return;

                            tcs.SetResult(new InstaResult<InstaProfile>()
                            {
                                Status = true,
                                Content = profile,
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                if (webView.Source.ToString() == requestUrl)
                {
                    webView.CoreWebView2.Reload();
                }
                else
                {
                    webView.Source = new Uri(requestUrl);
                }

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(waitInSeconds));
                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<InstaProfile>()
                {
                    Status = false,
                    Content = null,
                    Errors = new List<string>() { "Request timed out" }
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<InstaProfile>
                {
                    Status = false,
                    Content = null,
                    Errors = ex.GetAllInnerMessages(),
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceResponseReceived -= WebView_WebResourceResponseReceived;
            }
        }


        public static async Task<InstaResult<InstaPostVr2>> GetPostByShortCode(WebView2 webView, string postShortCode, int waitInSeconds = 60)
        {
            var requestUrl = $"https://www.instagram.com/p/{postShortCode}";
            return await GetPostByUrl(webView, requestUrl, waitInSeconds);
        }

        public static async Task<InstaResult<InstaPostVr2>> GetPostByUrl(WebView2 webView, string requestUrl, int waitInSeconds = 60)
        {
            var tcs = new TaskCompletionSource<InstaResult<InstaPostVr2>>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Equals(requestUrl);
                return check1;
            }

            JObject FindJsonNodeFromScripts(string html, string targetKey)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var scripts = doc.DocumentNode.SelectNodes("//script[@type='application/json']")?.Where(node => node.InnerText.Contains(targetKey));

                if (scripts == null || !scripts.Any()) return null;

                foreach (var script in scripts)
                {
                    try
                    {
                        var json = JObject.Parse(script.InnerText);
                        var result = FindNodeByKey(json, targetKey);
                        if (result != null)
                            return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("JSON Parse Error: " + ex.Message);
                    }
                }

                return null;
            }

            JObject FindNodeByKey(JToken token, string targetKey)
            {
                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    foreach (var prop in obj.Properties())
                    {
                        if (prop.Name == targetKey)
                            return prop.Value as JObject;

                        var found = FindNodeByKey(prop.Value, targetKey);
                        if (found != null)
                            return found;
                    }
                }
                else if (token.Type == JTokenType.Array)
                {
                    foreach (var item in (JArray)token)
                    {
                        var found = FindNodeByKey(item, targetKey);
                        if (found != null)
                            return found;
                    }
                }

                return null;
            }

            async void WebView_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request) || e.Response == null) return;

                    using (var stream = await e.Response.GetContentAsync())
                    {
                        if (stream == null) return;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            if (reader == null) return;
                            var content = await reader.ReadToEndAsync();
                            if (string.IsNullOrWhiteSpace(content)) return;

                            var obj = FindJsonNodeFromScripts(content, "xdt_api__v1__media__shortcode__web_info");
                            var edges = (JArray)obj["items"];
                            var node = edges[0].ToObject<InstaPostVr2>();

                            tcs.SetResult(new InstaResult<InstaPostVr2>()
                            {
                                Status = true,
                                Content = node,
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                if (webView.Source.ToString() == requestUrl)
                {
                    webView.CoreWebView2.Reload();
                }
                else
                {
                    webView.Source = new Uri(requestUrl);
                }

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(waitInSeconds));
                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<InstaPostVr2>()
                {
                    Status = false,
                    Content = null,
                    Errors = new List<string>() { "Request timed out" }
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<InstaPostVr2>
                {
                    Status = false,
                    Content = null,
                    Errors = ex.GetAllInnerMessages(),
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceResponseReceived -= WebView_WebResourceResponseReceived;
            }
        }


        public static async Task<InstaResult<List<InstaPost>>> GetPostsByUsername(WebView2 webView, string username, CancellationTokenSource cancellationToken, int postCount = 0, int minWait = 5, int maxWait = 15, EventHandler<InstaPostProgressArgs<InstaPost>> taskProgress = null, int loopBreakAttemps = 3)
        {
            var requestUrl = $"https://www.instagram.com/{username}";
            return await GetPostsByUrl(webView, requestUrl, cancellationToken, postCount, minWait, maxWait, taskProgress, loopBreakAttemps);
        }

        public static async Task<InstaResult<List<InstaPost>>> GetPostsByUrl(WebView2 webView, string requestUrl, CancellationTokenSource cancellationToken, int postCount = 0, int minWait = 5, int maxWait = 15, EventHandler<InstaPostProgressArgs<InstaPost>> taskProgress = null, int loopBreakAttemps = 3)
        {
            var requestFilter = "graphql/query";
            var allPosts = new List<InstaPost>();
            var random = new Random();
            var attempts = loopBreakAttemps;
            var requests = new Dictionary<string, bool>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals("X-Root-Field-Name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals("xdt_api__v1__feed__user_timeline_graphql_connection", StringComparison.CurrentCultureIgnoreCase));
                return check1 && check2;
            }

            async void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
                await Task.Delay(0);
                if (IsValidRequest(e.Request))
                {
                    var requestId = Guid.NewGuid().ToString();
                    e.Request.Headers.SetHeader("X-Request-Id", requestId);
                    requests.Add(requestId, false);
                }
            }

            async void OnWebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request)) return;

                    var headerReqId = e.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-Request-Id", StringComparison.CurrentCultureIgnoreCase));
                    requests[headerReqId.Value] = true;

                    attempts--;

                    using (var stream = await e.Response.GetContentAsync())
                    {
                        if (stream == null) return;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            if (reader == null) return;
                            var content = await reader.ReadToEndAsync();
                            if (string.IsNullOrWhiteSpace(content)) return;

                            var root = JObject.Parse(content);
                            var edges = (JArray)root["data"]?["xdt_api__v1__feed__user_timeline_graphql_connection"]?["edges"];
                            foreach (var edge in edges)
                            {
                                var node = (JObject)edge["node"];
                                if (node == null) continue;

                                var post = node.ToObject<InstaPost>();
                                if (post == null) continue;

                                allPosts.Add(post);
                            }

                            taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaPost>()
                            {
                                Message = $"{allPosts.Count} posts collected, attempts remaining {attempts}",
                            });
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            try
            {
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                if (webView.Source.ToString() == requestUrl)
                {
                    webView.CoreWebView2.Reload();
                }
                else
                {
                    webView.Source = new Uri(requestUrl);
                }

                while (!cancellationToken.IsCancellationRequested && allPosts.Count < postCount)
                {
                    if (requests.Values.Any(x => !x))
                    {
                        await Task.Delay(1000, cancellationToken.Token);
                        continue;
                    }

                    if (attempts == 0)
                    {
                        var wait = random.Next(15, 30);
                        attempts = loopBreakAttemps;

                        taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaPost>()
                        {
                            Message = $"Break loop wait for {wait} seconds...",
                            BreakLoop = true,
                            BreakLoopWait = random.Next(60, 180)
                        });
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken.Token);

                        continue;
                    }

                    WebViewHelper.ScrollDown(webView);

                    var timeWait = random.Next(minWait, maxWait);
                    taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaPost>()
                    {
                        Message = $"Wait for {timeWait} seconds...",
                    });

                    await Task.Delay(TimeSpan.FromSeconds(timeWait), cancellationToken.Token);
                }

                return new InstaResult<List<InstaPost>>()
                {
                    Status = true,
                    Content = allPosts,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaPost>>()
                {
                    Status = allPosts.Count > 0,
                    Content = allPosts,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }


        public static async Task<InstaResult<List<InstaFollowing>>> GetFollowingsByUsername(WebView2 webView, string username, CancellationTokenSource cancellationToken, int followingCount = 0, int minWait = 5, int maxWait = 15, EventHandler<InstaPostProgressArgs<InstaFollowing>> taskProgress = null, int loopBreakAttemps = 3)
        {
            var requestUrl = $"https://www.instagram.com/{username}/following/";
            return await GetFollowingsByUrl(webView, requestUrl, cancellationToken, followingCount, minWait, maxWait, taskProgress, loopBreakAttemps);
        }

        public static async Task<InstaResult<List<InstaFollowing>>> GetFollowingsByUrl(WebView2 webView, string requestUrl, CancellationTokenSource cancellationToken, int followingCount = 0, int minWait = 5, int maxWait = 15, EventHandler<InstaPostProgressArgs<InstaFollowing>> taskProgress = null, int loopBreakAttemps = 3)
        {
            var requestFilter = "api/v1/friendships";
            var random = new Random();
            var attempts = loopBreakAttemps;
            var requests = new Dictionary<string, bool>();
            var followings = new List<InstaFollowing>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                return check1;
            }

            async void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
                await Task.Delay(0);
                if (IsValidRequest(e.Request))
                {
                    var requestId = Guid.NewGuid().ToString();
                    e.Request.Headers.SetHeader("X-Request-Id", requestId);
                    requests.Add(requestId, false);
                }
            }

            async void OnWebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request)) return;

                    var headerReqId = e.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-Request-Id", StringComparison.CurrentCultureIgnoreCase));
                    requests[headerReqId.Value] = true;

                    attempts--;

                    using var stream = await e.Response.GetContentAsync();
                    if (stream == null) return;

                    using var reader = new StreamReader(stream);
                    if (reader == null) return;

                    var content = await reader.ReadToEndAsync();
                    if (string.IsNullOrWhiteSpace(content)) return;

                    var root = JObject.Parse(content);
                    var users = (JArray)root["users"];
                    if (users == null) return;

                    foreach (var user in users)
                    {
                        var node = (JObject)user;
                        if (node == null) continue;

                        var post = node.ToObject<InstaFollowing>();
                        if (post == null) continue;

                        followings.Add(post);
                    }

                    taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaFollowing>()
                    {
                        Message = $"{followings.Count} records collected, attempts remaining {attempts}",
                    });
                }
                catch (Exception)
                {

                }
            }

            try
            {
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                if (webView.Source.ToString() == requestUrl)
                    webView.CoreWebView2.Reload();
                else
                    webView.Source = new Uri(requestUrl);


                while (!cancellationToken.IsCancellationRequested && followings.Count < followingCount)
                {
                    if (requests.Values.Any(x => !x))
                    {
                        await Task.Delay(1000, cancellationToken.Token);
                        continue;
                    }

                    if (attempts == 0)
                    {
                        var wait = random.Next(15, 30);
                        attempts = loopBreakAttemps;

                        taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaFollowing>()
                        {
                            Message = $"Break loop wait for {wait} seconds...",
                            BreakLoop = true,
                            BreakLoopWait = random.Next(60, 180)
                        });
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken.Token);

                        continue;
                    }

                    WebViewHelper.ScrollDown(webView);

                    var timeWait = random.Next(minWait, maxWait);
                    taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaFollowing>()
                    {
                        Message = $"Wait for {timeWait} seconds...",
                    });

                    if (followings.Count >= followingCount) break;

                    await Task.Delay(TimeSpan.FromSeconds(timeWait), cancellationToken.Token);
                }

                return new InstaResult<List<InstaFollowing>>()
                {
                    Status = true,
                    Content = followings,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaFollowing>>()
                {
                    Status = followings.Count > 0,
                    Content = followings,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }

        public static async Task<InstaResult<List<InstaFollowing>>> GetFollowingsAjax(string userPk, string username, InstaBulkTaskParams<InstaFollowing> taskParams)
        {
            var scriptName = "AjaxGetFollowings.js";
            var collection = new List<InstaFollowing>();
            var successAttempts = 0;
            var failedAttempts = 0;
            var hasMore = false;
            var nextMaxId = 0;

            void SendTaskProgress(string message, bool breakLoop = false, int breakLoopWait = 0)
            {
                taskParams.TaskProgress?.Invoke(taskParams.WebView, new InstaPostProgressArgs<InstaFollowing>()
                {
                    Message = message,
                    BreakLoop = breakLoop,
                    BreakLoopWait = breakLoopWait
                });
            }

            void ProcessAjaxResult(bool resultStatus, string result)
            {
                if (!resultStatus)
                {
                    failedAttempts++;
                    SendTaskProgress(result);
                    return;
                }

                var root = JObject.Parse(result);
                if (root == null)
                {
                    failedAttempts++;
                    SendTaskProgress($"Root object is null, failed attempts {failedAttempts}");
                    return;
                }

                var status = root["status"]?.Value<bool>();
                if (status != true)
                {
                    failedAttempts++;
                    var error = root["error"]?.Value<string>();
                    SendTaskProgress($"{error ?? "Unknown error"}, {failedAttempts}");
                    return;
                }

                var users = (JArray)root["data"]?["users"];
                if (users == null)
                {
                    failedAttempts++;
                    SendTaskProgress($"Unable to find users, {failedAttempts}");
                    return;
                }

                hasMore = root["data"]?["has_more"]?.Value<bool>() ?? false;
                nextMaxId = root["data"]?["next_max_id"]?.Value<int>() ?? 0;
                var list = new List<InstaFollowing>();

                foreach (var user in users)
                {
                    try
                    {
                        var item = (user as JObject)?.ToObject<InstaFollowing>();
                        if (item == null) continue;

                        list.Add(item);
                    }
                    catch (Exception)
                    { }
                }

                successAttempts++;
                collection.AddRange(list);
                SendTaskProgress($"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
            }

            async Task CallAjaxAsync()
            {
                var requestId = Guid.NewGuid().ToString();
                var script = nextMaxId == 0 ?
                    $"fetchFollowing('{requestId}','{userPk}','{username}')" :
                    $"fetchFollowing('{requestId}','{userPk}','{username}', {nextMaxId})";

                var (status, result) = await WebViewHelper.ExecuteScriptForResult(taskParams.WebView, script);
                ProcessAjaxResult(status, result);
            }

            try
            {
                var (status, scriptText) = GetScriptFromFile(scriptName);
                if(!status)
                {
                    return new InstaResult<List<InstaFollowing>>()
                    {
                        Status = false,
                        Content = collection,
                        Errors = [$"{scriptName} not found"]
                    };
                }

                await taskParams.WebView.ExecuteScriptAsync(scriptText);

                while (!taskParams.CancellationToken.IsCancellationRequested)
                {
                    await CallAjaxAsync();

                    if (hasMore == false) break;
                    if (collection.Count >= taskParams.RecordsCount) break;
                    if (failedAttempts == taskParams.FailedAttempts) break;
                    if (successAttempts % taskParams.LoopBreakAttempts == 0)
                    {
                        var wait = random.Next(15, 60);
                        SendTaskProgress($"Break loop wait for {wait} seconds...", true, wait);
                        await Task.Delay(TimeSpan.FromSeconds(wait), taskParams.CancellationToken.Token);
                        continue;
                    }

                    var timeWait = random.Next(taskParams.MinWait, taskParams.MaxWait);
                    SendTaskProgress($"Wait for {timeWait} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(timeWait), taskParams.CancellationToken.Token);
                }

                return new InstaResult<List<InstaFollowing>>()
                {
                    Status = true,
                    Content = collection,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaFollowing>>()
                {
                    Status = collection.Count > 0,
                    Content = collection,
                    Errors = ex.GetAllInnerMessages()
                };
            }
        }


        public static async Task<InstaResult<List<InstaComment>>> GetPostComments(WebView2 webView, string postShortCode, CancellationTokenSource cancellationToken, int postCount = 0, int minWait = 5, int maxWait = 15, EventHandler<InstaPostProgressArgs<InstaComment>> taskProgress = null, int loopBreakAttemps = 3)
        {
            var requestFilter = "graphql/query";
            var requestUrl = $"https://www.instagram.com/p/{postShortCode}/comments/";
            var responseFilterKey = "X-Root-Field-Name".ToLower();
            var responseFilterValue = "xdt_api__v1__media__media_id__comments__connection".ToLower();
            var responseFilterValue2 = "xdt_api__v1__web__accounts__get_encrypted_credentials".ToLower();
            var requestCount = 0;
            var responseCount = 0;
            var retryAttempts = 5;
            var allComments = new List<InstaComment>();
            var requestCounter = new Dictionary<string, bool>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.ToLower() == responseFilterKey && x.Value.ToLower() == responseFilterValue);
                var check3 = Request.Uri == requestUrl;
                return (check1 && check2) || check3;
            }

            async void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
                await Task.Delay(0);
                if (IsValidRequest(e.Request))
                {
                    requestCount++;
                    var id = Guid.NewGuid().ToString();
                    e.Request.Headers.SetHeader("x-request-id", id);
                    requestCounter.Add(id, false);
                }
            }

            async void OnWebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request) || e.Response == null) return;

                    var hasRequestId = e.Request.Headers.Any(x => x.Key == "x-request-id");
                    if (hasRequestId)
                    {
                        var header = e.Request.Headers.First(x => x.Key == "x-request-id");
                        requestCounter[header.Value] = true;
                    }

                    responseCount++;

                    using (var stream = await e.Response.GetContentAsync())
                    {
                        if (stream == null) return;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            if (reader == null) return;
                            var content = await reader.ReadToEndAsync();
                            if (string.IsNullOrWhiteSpace(content)) return;

                            if (e.Request.Uri == requestUrl)
                            {
                                var obj = FindJsonNodeFromScripts(content, responseFilterValue, responseFilterValue);
                                var edges = (JArray)obj["edges"];
                                foreach (var edge in edges)
                                {
                                    var node = (JObject)edge["node"];
                                    if (node == null) continue;

                                    var comment = node.ToObject<InstaComment>();
                                    if (comment == null) continue;

                                    allComments.Add(comment);
                                }
                            }
                            else
                            {
                                var root = JObject.Parse(content);
                                var edges = (JArray)root["data"]?["xdt_api__v1__media__media_id__comments__connection"]?["edges"];
                                foreach (var edge in edges)
                                {
                                    var node = (JObject)edge["node"];
                                    if (node == null) continue;

                                    var comment = node.ToObject<InstaComment>();
                                    if (comment == null) continue;

                                    allComments.Add(comment);
                                }
                            }

                            taskProgress?.Invoke(webView, new InstaPostProgressArgs<InstaComment>()
                            {
                                Data = allComments
                            });
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            JObject FindJsonNodeFromScripts(string html, string keyword, string targetKey)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var scripts = doc.DocumentNode.SelectNodes("//script[@type='application/json']")?.Where(node => node.InnerText.Contains(keyword));

                if (scripts == null || !scripts.Any()) return null;

                foreach (var script in scripts)
                {
                    try
                    {
                        var json = JObject.Parse(script.InnerText);
                        var result = FindNodeByKey(json, targetKey);
                        if (result != null)
                            return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("JSON Parse Error: " + ex.Message);
                    }
                }

                return null;
            }

            JObject FindNodeByKey(JToken token, string targetKey)
            {
                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    foreach (var prop in obj.Properties())
                    {
                        if (prop.Name == targetKey)
                            return prop.Value as JObject;

                        var found = FindNodeByKey(prop.Value, targetKey);
                        if (found != null)
                            return found;
                    }
                }
                else if (token.Type == JTokenType.Array)
                {
                    foreach (var item in (JArray)token)
                    {
                        var found = FindNodeByKey(item, targetKey);
                        if (found != null)
                            return found;
                    }
                }

                return null;
            }

            try
            {
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                if (webView.Source.ToString() == requestUrl)
                {
                    webView.CoreWebView2.Reload();
                }
                else
                {
                    webView.Source = new Uri(requestUrl);
                }

                while (true)
                {
                    if (requestCounter.Any(x => x.Value == false))
                    {
                        retryAttempts = 5;
                    }
                    else
                    {
                        var scrollDuration = new Random().Next(5, 10);
                        WebViewHelper.ScrollDownAllDivOnPage(webView, scrollDuration);
                        retryAttempts--;
                    }

                    //if (cancellationToken.IsCancellationRequested || allComments.Count >= commentCount || retryAttempts <= 0)
                    //{
                    //    break;
                    //}

                    await Task.Delay(5000);
                }

                return new InstaResult<List<InstaComment>>()
                {
                    Status = true,
                    Content = allComments,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaComment>>()
                {
                    Status = allComments.Count > 0,
                    Content = allComments,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                webView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }
    }

    ////////////////////////////////////////////////

    public class InstaUser
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
        public object edge_web_feed_timeline { get; set; }
    }

    public class WebViewJsExecuteResult
    {
        public string Status { get; set; }
        public string ResultContent { get; set; }
        public List<string> Errors { get; set; }
    }

    public class InstaResult<T> where T : class
    {
        public bool Status { get; set; }
        public T Content { get; set; }
        public WebViewJsExecuteResult JsResult { get; set; }
        public List<string> Errors { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaProfile
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string username { get; set; }
        public string profile_pic_url { get; set; }
        public string full_name { get; set; }
        public string category { get; set; }
        public int? follower_count { get; set; }
        public int? following_count { get; set; }
        public int? media_count { get; set; }
        public List<InstaBioLink> bio_links { get; set; }
        public InstaLinkedFbInfo linked_fb_info { get; set; }
        public string biography { get; set; }
        public string address_street { get; set; }
        public string city_name { get; set; }
        public bool? is_business { get; set; }
        public string zip { get; set; }
        public string external_lynx_url { get; set; }
        public string external_url { get; set; }

        public bool Validate()
        {
            var check1 = !string.IsNullOrEmpty(this.username);
            var check2 = !string.IsNullOrEmpty(this.category);
            var check3 = !string.IsNullOrEmpty(this.id);

            return check1 && check2 && check3;
        }
    }

    public class InstaBioLink
    {
        public string image_url { get; set; }
        public bool? is_pinned { get; set; }
        public string link_type { get; set; }
        public string lynx_url { get; set; }
        public string media_type { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string creation_source { get; set; }
    }

    public class InstaLinkedFbInfo
    {
        public object linked_fb_page { get; set; }
        public InstaLinkedFbUser linked_fb_user { get; set; }
    }

    public class InstaLinkedFbUser
    {
        public string name { get; set; }
        public string profile_url { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaPost
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string code { get; set; }
        public InstaCaption caption { get; set; }
        public string product_type { get; set; }
        public string organic_tracking_token { get; set; }
        public long? taken_at { get; set; }
        public long? comment_count { get; set; }
        public bool? comments_disabled { get; set; }
        public long? like_count { get; set; }
        public int? fb_like_count { get; set; }
        public InstaPostLocation location { get; set; }
        public List<IntaTopLiker> facepile_top_likers { get; set; }
        public List<InstaVideoVersion> video_versions { get; set; }
        public DateTime? created_at
        {
            get
            {

                return taken_at == null ? null : (DateTime?)DateTimeOffset.FromUnixTimeSeconds(taken_at.Value).UtcDateTime;
            }
        }
    }

    public class InstaCaption
    {
        public long? CreatedAt { get; set; }
        public string Pk { get; set; }
        public string Text { get; set; }
    }

    public class InstaPostLocation
    {
        public string Pk { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Name { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Typename { get; set; }
    }

    public class IntaTopLiker
    {
        public string ProfilePicUrl { get; set; }
        public string Pk { get; set; }
        public string Username { get; set; }
        public string Id { get; set; }
    }

    public class InstaVideoVersion
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Url { get; set; }
        public int? Type { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaComment
    {
        public string pk { get; set; }
        public int? child_comment_count { get; set; }
        public bool? has_liked_comment { get; set; }
        public string text { get; set; }
        public long? created_at { get; set; }
        public string parent_comment_id { get; set; }
        public int comment_like_count { get; set; }
        public InstaCommentUser user { get; set; }
    }

    public class InstaCommentUser
    {
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaFollowing
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string pk { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaReel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string reel_type { get; set; }
        public bool has_besties_media { get; set; }
        public bool muted { get; set; }
        public long latest_reel_media { get; set; }
        public long seen { get; set; }
        public long expiring_at { get; set; }
        public int ranked_position { get; set; }
        public int seen_ranked_position { get; set; }
        public InstaReelUser user { get; set; }
        public string __typename { get; set; }
    }

    public class InstaReelUser
    {
        public string pk { get; set; }
        public string username { get; set; }
        public string live_broadcast_visibility { get; set; }
        public string live_broadcast_id { get; set; }
        public string profile_pic_url { get; set; }
        public bool? is_unpublished { get; set; }
        public string id { get; set; }
        public string hd_profile_pic_url_info { get; set; }
        public long? latest_reel_media { get; set; }
        public long? reel_media_seen_timestamp { get; set; }
    }

    ///////////////////////////////////

    public class InstaPostVr2
    {
        public string code { get; set; }
        public string pk { get; set; }
        public string id { get; set; }
        public long taken_at { get; set; }
        public List<InstaVideoVersion> video_versions { get; set; }
        public InstaImageVersions2 image_versions2 { get; set; }
        public InstaUserVr2 user { get; set; }
        public string product_type { get; set; }
        public InstaUserTags usertags { get; set; }
        public InstaLocation location { get; set; }
        public int like_count { get; set; }
        public InstaOwner owner { get; set; }
        public int comment_count { get; set; }
        public List<string> top_likers { get; set; }
        public int fb_like_count { get; set; }
        public InstaCaption caption { get; set; }
    }

    public class InstaImageVersions2
    {
        public List<InstaCandidate> candidates { get; set; }
    }

    public class InstaCandidate
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class InstaUserVr2
    {
        public string pk { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public bool is_private { get; set; }
        public bool is_embeds_disabled { get; set; }
        public bool is_unpublished { get; set; }
        public bool is_verified { get; set; }
        public InstaFriendshipStatus friendship_status { get; set; }
        public int latest_reel_media { get; set; }
        public string id { get; set; }
        public string __typename { get; set; }
        public object live_broadcast_visibility { get; set; }
        public object live_broadcast_id { get; set; }
        public InstaHdProfilePicUrlInfo hd_profile_pic_url_info { get; set; }
    }

    public class InstaFriendshipStatus
    {
        public object blocking { get; set; }
        public bool followed_by { get; set; }
        public bool following { get; set; }
        public object incoming_request { get; set; }
        public bool is_private { get; set; }
        public bool is_restricted { get; set; }
        public object is_viewer_unconnected { get; set; }
        public object muting { get; set; }
        public object outgoing_request { get; set; }
        public object subscribed { get; set; }
        public bool is_feed_favorite { get; set; }
    }

    public class InstaHdProfilePicUrlInfo
    {
        public string url { get; set; }
    }

    public class InstaUserTags
    {
        public List<InstaUserTagIn> @in { get; set; }
    }

    public class InstaUserTagIn
    {
        public InstaTaggedUser user { get; set; }
        public List<int> position { get; set; }
    }

    public class InstaTaggedUser
    {
        public string pk { get; set; }
        public string full_name { get; set; }
        public string username { get; set; }
        public string profile_pic_url { get; set; }
        public bool is_verified { get; set; }
        public string id { get; set; }
    }

    public class InstaLocation
    {
        public long pk { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string name { get; set; }
        public object profile_pic_url { get; set; }
        public string __typename { get; set; }
    }

    public class InstaOwner
    {
        public string pk { get; set; }
        public string id { get; set; }
        public string username { get; set; }
        public string profile_pic_url { get; set; }
        public bool show_account_transparency_details { get; set; }
        public string __typename { get; set; }
        public bool is_private { get; set; }
        public object transparency_product { get; set; }
        public bool transparency_product_enabled { get; set; }
        public object transparency_label { get; set; }
        public object ai_agent_owner_username { get; set; }
        public bool is_unpublished { get; set; }
        public bool is_verified { get; set; }
    }

    ///////////////////////////////////

    public class InstaPostProgressArgs<T> where T : class
    {
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public bool BreakLoop { get; set; }
        public int BreakLoopWait { get; set; }
    }

    ///////////////////////////////////

    public class InstaBulkTaskParams<T> where T : class, new()
    {
        public WebView2 WebView { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public int RecordsCount { get; set; } = 0;
        public int MinWait { get; set; } = 5;
        public int MaxWait { get; set; } = 15;
        public EventHandler<InstaPostProgressArgs<T>> TaskProgress { get; set; }
        public int LoopBreakAttempts { get; set; } = 3;
        public int FailedAttempts { get; set; } = 3;
    }

    ///////////////////////////////////

}
