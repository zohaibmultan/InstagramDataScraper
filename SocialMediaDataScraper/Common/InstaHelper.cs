#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace SocialMediaDataScraper.Models
{
    public class InstaHelper
    {
        private readonly Random random = new();
        private bool isLogin = false;

        private (bool, string) GetScriptFromFile(string scriptFileName)
        {
            var scriptPath = Path.Combine(WebViewHelper.ScriptDirectory, scriptFileName);
            if (!Path.Exists(scriptPath)) return (false, $"{scriptFileName} not found");

            var scriptText = File.ReadAllText(scriptPath);
            if (string.IsNullOrEmpty(scriptText)) return (false, $"{scriptFileName} is empty");

            return (true, scriptText);
        }

        private (JObject, List<string>) FindJsonNodeFromScripts(string html, string keyword)
        {
            ArgumentNullException.ThrowIfNull(html, nameof(html));
            ArgumentNullException.ThrowIfNull(keyword, nameof(keyword));

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var scripts = doc.DocumentNode
                .SelectNodes("//script[@type='application/json']")?
                .Where(node => node.InnerText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (scripts == null || scripts.Count == 0) return (null, ["Keywords not found in script"]);

            foreach (var script in scripts)
            {
                try
                {
                    var json = JObject.Parse(script.InnerText);
                    var result = FindNodeByKey(json, keyword);
                    if (result != null)
                        return (result, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JSON Parse Error: " + ex.Message);
                }
            }

            return (null, ["Jons not found in script"]);
        }

        private JObject FindNodeByKey(JToken token, string targetKey)
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

        private void NavigateOrReload(WebView2 webView, string requestUrl)
        {
            if (webView.Source.ToString().Equals(requestUrl, StringComparison.CurrentCultureIgnoreCase))
            {
                webView.CoreWebView2.Reload();
            }
            else
            {
                webView.Source = new Uri(requestUrl);
            }
        }

        private void SendTaskProgress<T>(InstaBulkTaskParams<T> taskParams, string message, bool breakLoop = false, TimeSpan? breakLoopWait = null) where T : class, new()
        {
            taskParams.TaskProgress?.Invoke(taskParams.WebView, new InstaPostProgressArgs<T>()
            {
                Message = message,
                BreakLoop = breakLoop,
                BreakLoopWait = breakLoopWait ?? TimeSpan.Zero
            });
        }


        private async Task<(bool, string, List<string>)> GetWebResponseContent(CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            var errors = new List<string>();
            if (e.Response == null)
            {
                errors.Add("Response is null");
                return (false, null, errors);
            }

            using var stream = await e.Response.GetContentAsync();
            if (stream == null)
            {
                errors.Add("Response stream is null");
                return (false, null, errors);
            }

            using var reader = new StreamReader(stream);
            if (reader == null)
            {
                errors.Add("Response stream reader is null");
                return (false, null, errors);
            }

            var content = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                errors.Add("Response stream content is null");
                return (false, null, errors);
            }

            return (true, content, errors);
        }

        private async Task<(bool, JObject, List<string>)> GetWebResponseJsonObject(CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            var (status, content, errors) = await GetWebResponseContent(e);
            if (!status) return (status, null, errors);

            var root = JObject.Parse(content);
            if (root == null)
            {
                errors.Add("Root object is null");
                return (false, null, errors);
            }

            return (true, root, errors);
        }

        private async Task<WebViewJsExecuteResult> ExecuteJsAsync(WebView2 webView, string jsCode, int waitInSeconds = 15)
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


        public async Task<InstaResult<string>> TestLogin(WebView2 webView, string username, TimeSpan? waitInSeconds = null)
        {
            var requestFilter = "graphql/query";
            var requestUrl = $"https://www.instagram.com/{username}";
            var responseFilterKey = "X-Root-Field-Name";
            var responseFilterValue = "xdt_api__v1__feed__reels_tray";
            var tcs = new TaskCompletionSource<InstaResult<string>>();
            var cts = new CancellationTokenSource(waitInSeconds ?? TimeSpan.FromSeconds(60));
            var errors = new List<string>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals(responseFilterKey, StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals(responseFilterValue, StringComparison.CurrentCultureIgnoreCase));
                return check1 && check2;
            }

            async void WebView_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                try
                {
                    if (!IsValidRequest(e.Request)) return;

                    var (status, rootObject, errors) = await GetWebResponseJsonObject(e);
                    if (!status)
                    {
                        tcs.TrySetResult(new() { Status = false, Errors = errors });
                        return;
                    }

                    var feed = rootObject?["data"]?[responseFilterValue]?["tray"]?.ToObject<List<InstaReel>>();
                    if (feed == null)
                    {
                        tcs.TrySetResult(new() { Status = false, Errors = ["Required node not found"] });
                        return;
                    }

                    var userPk = rootObject?["data"]?["xdt_viewer"]?["user"]?["id"]?.ToString();
                    isLogin = true;

                    tcs.SetResult(new()
                    {
                        Status = true,
                        Content = userPk,
                    });
                }
                catch (Exception ex)
                {
                    errors = [.. errors, .. ex.GetAllInnerMessages()];
                    return;
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                NavigateOrReload(webView, requestUrl);

                cts.Token.Register(() => tcs.TrySetResult(new()
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, "Request timed out"],
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<string>
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, .. ex.GetAllInnerMessages()],
                };
            }
            finally
            {
                if (webView?.CoreWebView2 != null)
                {
                    webView.CoreWebView2.WebResourceResponseReceived -= WebView_WebResourceResponseReceived;
                }

                cts.Dispose();
            }
        }


        public async Task<InstaResult<InstaProfile>> GetProfileByUsername(WebView2 webView, string username, TimeSpan? waitInSeconds = null)
        {
            var requestUrl = $"https://www.instagram.com/{username}";
            return await GetProfileByUrl(webView, requestUrl, waitInSeconds);
        }

        public async Task<InstaResult<InstaProfile>> GetProfileByUrl(WebView2 webView, string requestUrl, TimeSpan? waitInSeconds = null)
        {
            var username = new Uri(requestUrl).AbsolutePath.Trim('/');
            var requestFilter = "graphql/query";
            var tcs = new TaskCompletionSource<InstaResult<InstaProfile>>();
            var cts = new CancellationTokenSource(waitInSeconds ?? TimeSpan.FromSeconds(15));
            var errors = new List<string>();

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
                    if (!IsValidRequest(e.Request)) return;
                    var (status, rootObject, errors) = await GetWebResponseJsonObject(e);
                    if (!status) return;

                    var profile = rootObject["data"]?["user"]?.ToObject<InstaProfile>();
                    if (profile == null || !profile.Validate())
                    {
                        errors.Add("Required node not found");
                        return;
                    }

                    tcs.SetResult(new InstaResult<InstaProfile>()
                    {
                        Status = true,
                        Content = profile,
                    });
                }
                catch (Exception ex)
                {
                    errors = [.. errors, .. ex.GetAllInnerMessages()];
                    return;
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                NavigateOrReload(webView, requestUrl);

                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<InstaProfile>()
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, "Request timed out"]
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<InstaProfile>
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, .. ex.GetAllInnerMessages()],
                };
            }
            finally
            {
                webView.CoreWebView2.WebResourceResponseReceived -= WebView_WebResourceResponseReceived;
                cts.Dispose();
            }
        }


        public async Task<InstaResult<InstaPostVr2>> GetPostByShortCode(WebView2 webView, string postShortCode, TimeSpan? waitInSeconds = null)
        {
            var requestUrl = $"https://www.instagram.com/p/{postShortCode}";
            return await GetPostByUrl(webView, requestUrl, waitInSeconds);
        }

        public async Task<InstaResult<InstaPostVr2>> GetPostByUrl(WebView2 webView, string requestUrl, TimeSpan? waitInSeconds = null)
        {
            var tcs = new TaskCompletionSource<InstaResult<InstaPostVr2>>();
            var cts = new CancellationTokenSource(waitInSeconds ?? TimeSpan.FromSeconds(15));
            var errors = new List<string>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Equals(requestUrl);
                return check1;
            }

            async void WebView_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
            {
                if (e?.Request is null || !IsValidRequest(e.Request)) return;

                try
                {
                    var (status, content, errors) = await GetWebResponseContent(e);
                    if (!status || string.IsNullOrEmpty(content))
                    {
                        tcs.TrySetResult(new InstaResult<InstaPostVr2> { Status = false, Errors = errors });
                        return;
                    }

                    var (obj, parseErrors) = FindJsonNodeFromScripts(content, "xdt_api__v1__media__shortcode__web_info");
                    if (obj is null)
                    {
                        tcs.TrySetResult(new InstaResult<InstaPostVr2> { Status = false, Errors = parseErrors });
                        return;
                    }

                    var edges = obj["items"] as JArray;
                    if (edges is null || !edges.Any())
                    {
                        tcs.TrySetResult(new InstaResult<InstaPostVr2> { Status = false, Errors = ["No items found in JSON response"] });
                        return;
                    }

                    var node = edges[0].ToObject<InstaPostVr2>();
                    tcs.TrySetResult(new InstaResult<InstaPostVr2>
                    {
                        Status = true,
                        Content = node
                    });
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(new InstaResult<InstaPostVr2>
                    {
                        Status = false,
                        Errors = ex.GetAllInnerMessages(),
                    });
                }
            }

            try
            {
                webView.CoreWebView2.WebResourceResponseReceived += WebView_WebResourceResponseReceived;
                NavigateOrReload(webView, requestUrl);

                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<InstaPostVr2>()
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, "Request timed out"]
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
                cts.Dispose();
            }
        }


        public async Task<InstaResult<List<InstaPost>>> GetPostsByUsername(string username, InstaBulkTaskParams<InstaPost> taskParams)
        {
            var requestUrl = $"https://www.instagram.com/{username}";
            return await GetPostsByUrl(requestUrl, taskParams);
        }

        public async Task<InstaResult<List<InstaPost>>> GetPostsByUrl(string requestUrl, InstaBulkTaskParams<InstaPost> taskParams)
        {
            var username = new Uri(requestUrl).AbsolutePath.Trim('/');
            var requestFilter = "graphql/query";
            var collection = new List<InstaPost>();
            var requests = new Dictionary<string, bool>();
            var errors = new List<string>();
            var random = new Random();
            var successAttempts = 0;
            var failedAttempts = 0;
            var hasNextPage = true;

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals("X-Root-Field-Name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals("xdt_api__v1__feed__user_timeline_graphql_connection", StringComparison.CurrentCultureIgnoreCase));
                return check1 && check2;
            }

            void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
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
                    if (e?.Request is null || !IsValidRequest(e.Request)) return;

                    var headerReqId = e.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-Request-Id", StringComparison.CurrentCultureIgnoreCase));
                    if (headerReqId.Value is null)
                    {
                        failedAttempts++;
                        errors.Add("Request validated but not registered");
                        return;
                    }

                    requests[headerReqId.Value] = true;

                    var (status, rootObject, parseErrors) = await GetWebResponseJsonObject(e);
                    if (!status)
                    {
                        failedAttempts++;
                        errors.AddRange(parseErrors);
                        return;
                    }

                    var edges = rootObject?["data"]?["xdt_api__v1__feed__user_timeline_graphql_connection"]?["edges"] as JArray;
                    if (edges is null || !edges.Any())
                    {
                        failedAttempts++;
                        errors.Add("Required node edges not found");
                        return;
                    }

                    var list = edges.Select(edge => edge["node"]?.ToObject<InstaPost>())
                        .Where(post => post is not null)
                        .ToList();

                    if (list.Count > 0)
                    {
                        successAttempts++;
                        failedAttempts = 0;
                        errors.Clear();
                        collection.AddRange(list);
                        SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
                    }
                    else
                    {
                        failedAttempts++;
                        errors.Add("Required node edges not found");
                    }
                }
                catch (Exception ex)
                {
                    failedAttempts++;
                    errors = [.. errors, .. ex.GetAllInnerMessages()];
                }
            }

            try
            {
                taskParams.WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                taskParams.WebView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                NavigateOrReload(taskParams.WebView, requestUrl);

                while (!taskParams.CancellationToken.IsCancellationRequested)
                {
                    WebViewHelper.ScrollDown(taskParams.WebView);

                    if (!hasNextPage) break;
                    if (taskParams.RecordsCount != 0 && collection.Count >= taskParams.RecordsCount) break;
                    if (failedAttempts == taskParams.FailedAttempts) break;
                    if (successAttempts > 0 && successAttempts % taskParams.LoopBreakAttempts == 0)
                    {
                        var wait = random.Next(15, 60);
                        SendTaskProgress(taskParams, $"Break loop wait for {wait} seconds...", true, TimeSpan.FromSeconds(wait));
                        await Task.Delay(TimeSpan.FromSeconds(wait), taskParams.CancellationToken.Token);
                        continue;
                    }

                    var timeWait = random.Next(taskParams.MinWait, taskParams.MaxWait);
                    SendTaskProgress(taskParams, $"Wait for {timeWait} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(timeWait), taskParams.CancellationToken.Token);
                }

                collection.ForEach(x => x.username = username);

                return new InstaResult<List<InstaPost>>()
                {
                    Status = true,
                    Content = collection,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaPost>>()
                {
                    Status = collection.Count > 0,
                    Content = collection,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                taskParams.WebView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }


        public async Task<InstaResult<List<InstaFollowing>>> GetFollowingsByUsername(string username, InstaBulkTaskParams<InstaFollowing> taskParams)
        {
            var requestUrl = $"https://www.instagram.com/{username}/following/";
            return await GetFollowingsByUrl(requestUrl, taskParams);
        }

        public async Task<InstaResult<List<InstaFollowing>>> GetFollowingsByUrl(string requestUrl, InstaBulkTaskParams<InstaFollowing> taskParams)
        {
            var requestFilter = "api/v1/friendships";
            var requests = new Dictionary<string, bool>();
            var collection = new List<InstaFollowing>();
            var errors = new List<string>();
            var random = new Random();
            var successAttempts = 0;
            var failedAttempts = 0;
            var hasNextPage = true;

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                return check1;
            }

            void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
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
                    if (e?.Response is null || !IsValidRequest(e.Request)) return;

                    var headerReqId = e.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-Request-Id", StringComparison.CurrentCultureIgnoreCase));
                    if (headerReqId.Value is null)
                    {
                        failedAttempts++;
                        errors.Add("Request validated but not registered");
                        return;
                    }

                    requests[headerReqId.Value] = true;

                    var (status, rootObject, parseErrors) = await GetWebResponseJsonObject(e);
                    if (!status)
                    {
                        failedAttempts++;
                        errors.AddRange(parseErrors);
                        return;
                    }

                    var edges = rootObject["users"] as JArray;
                    if (edges is null || !edges.Any())
                    {
                        failedAttempts++;
                        errors.Add("Required node edges not found");
                        return;
                    }

                    var list = edges.Select(edge => edge["node"]?.ToObject<InstaFollowing>())
                        .Where(post => post is not null)
                        .ToList();

                    if (list.Count > 0)
                    {
                        successAttempts++;
                        failedAttempts = 0;
                        errors.Clear();
                        collection.AddRange(list);
                        SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
                    }
                    else
                    {
                        failedAttempts++;
                        errors.Add("Required node edges not found");
                    }
                }
                catch (Exception ex)
                {
                    failedAttempts++;
                    errors = [.. errors, .. ex.GetAllInnerMessages()];
                }
            }

            try
            {
                taskParams.WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                taskParams.WebView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                NavigateOrReload(taskParams.WebView, requestUrl);

                while (!taskParams.CancellationToken.IsCancellationRequested)
                {
                    WebViewHelper.ScrollDown(taskParams.WebView);

                    if (!hasNextPage) break;
                    if (taskParams.RecordsCount != 0 && collection.Count >= taskParams.RecordsCount) break;
                    if (failedAttempts == taskParams.FailedAttempts) break;
                    if (successAttempts > 0 && successAttempts % taskParams.LoopBreakAttempts == 0)
                    {
                        var wait = random.Next(15, 60);
                        SendTaskProgress(taskParams, $"Break loop wait for {wait} seconds...", true, TimeSpan.FromSeconds(wait));
                        await Task.Delay(TimeSpan.FromSeconds(wait), taskParams.CancellationToken.Token);
                        continue;
                    }

                    var timeWait = random.Next(taskParams.MinWait, taskParams.MaxWait);
                    SendTaskProgress(taskParams, $"Wait for {timeWait} seconds...");
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
            finally
            {
                taskParams.WebView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }

        public async Task<InstaResult<List<InstaFollowing>>> GetFollowingsAjax(string userPk, string username, InstaBulkTaskParams<InstaFollowing> taskParams)
        {
            var scriptName = "AjaxGetFollowings.js";
            var collection = new List<InstaFollowing>();
            var successAttempts = 0;
            var failedAttempts = 0;
            var nextMaxId = 0;

            void ProcessAjaxResult(bool resultStatus, string result)
            {
                if (!resultStatus)
                {
                    failedAttempts++;
                    SendTaskProgress(taskParams, result);
                    return;
                }

                var root = JObject.Parse(result);
                if (root == null)
                {
                    failedAttempts++;
                    SendTaskProgress(taskParams, $"Root object is null, failed attempts {failedAttempts}");
                    return;
                }

                var status = root["status"]?.Value<bool>();
                if (status != true)
                {
                    failedAttempts++;
                    var error = root["error"]?.Value<string>();
                    SendTaskProgress(taskParams, $"{error ?? "Unknown error"}, {failedAttempts}");
                    return;
                }

                var users = (JArray)root["data"]?["users"];
                if (users == null)
                {
                    failedAttempts++;
                    SendTaskProgress(taskParams, $"Unable to find users, {failedAttempts}");
                    return;
                }

                nextMaxId = root["data"]?["next_max_id"]?.Value<int>() ?? -1;
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
                SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
            }

            async Task CallAjaxAsync()
            {
                var requestId = Guid.NewGuid().ToString();
                var script = nextMaxId == 0 ?
                    $"fetchFollowing('{requestId}','{userPk}','{username}')" :
                    $"fetchFollowing('{requestId}','{userPk}','{username}', {nextMaxId})";

                var (status, result) = await WebViewHelper.ExecuteScriptForResult(taskParams.WebView, script, taskParams.CancellationToken.Token);
                ProcessAjaxResult(status, result);
            }

            try
            {
                var (status, scriptText) = GetScriptFromFile(scriptName);
                if (!status)
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

                    if (nextMaxId == -1) break;
                    if (collection.Count >= taskParams.RecordsCount) break;
                    if (failedAttempts == taskParams.FailedAttempts) break;
                    if (successAttempts % taskParams.LoopBreakAttempts == 0)
                    {
                        var wait = random.Next(15, 60);
                        SendTaskProgress(taskParams, $"Break loop wait for {wait} seconds...", true, TimeSpan.FromSeconds(wait));
                        await Task.Delay(TimeSpan.FromSeconds(wait), taskParams.CancellationToken.Token);
                        continue;
                    }

                    var timeWait = random.Next(taskParams.MinWait, taskParams.MaxWait);
                    SendTaskProgress(taskParams, $"Wait for {timeWait} seconds...");
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


        public async Task<InstaResult<List<InstaComment>>> GetPostComments(string postShortCode, InstaBulkTaskParams<InstaComment> taskParams)
        {
            var requestFilter = "graphql/query";
            var requestUrl = $"https://www.instagram.com/p/{postShortCode}/comments/";
            var responseFilterKey = "X-Root-Field-Name";
            var responseFilterValue = "xdt_api__v1__media__media_id__comments__connection";
            var collection = new List<InstaComment>();
            var requests = new Dictionary<string, bool>();
            var errors = new List<string>();
            var successAttempts = 0;
            var failedAttempts = 0;
            var hasNextPage = true;

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals(responseFilterKey, StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals(responseFilterValue, StringComparison.CurrentCultureIgnoreCase));
                var check3 = Request.Uri == requestUrl;
                return (check1 && check2) || check3;
            }

            void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
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
                    if (!IsValidRequest(e.Request) || e.Response == null) return;

                    var headerReqId = e.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-Request-Id", StringComparison.CurrentCultureIgnoreCase));
                    if (headerReqId.Value is null)
                    {
                        failedAttempts++;
                        errors.Add("Request validated but not registered");
                        return;
                    }

                    requests[headerReqId.Value] = true;

                    var (status, content, parseErrors) = await GetWebResponseContent(e);
                    if (!status)
                    {
                        failedAttempts++;
                        errors.AddRange(parseErrors);
                        return;
                    }

                    if (e.Request.Uri == requestUrl)
                    {
                        var (obj, scriptParseErrors) = FindJsonNodeFromScripts(content, responseFilterValue);
                        if (obj is null)
                        {
                            failedAttempts++;
                            SendTaskProgress(taskParams, $"Required keywords not found on page script tag, failed {failedAttempts}");
                            taskParams.CancellationToken.Cancel();
                            return;
                        }

                        hasNextPage = obj["page_info"]?["has_next_page"].Value<bool>() ?? false;
                        var edges = obj["edges"] as JArray;
                        if (edges is null || !edges.Any())
                        {
                            failedAttempts++;
                            SendTaskProgress(taskParams, $"Unable to get edges, failed {failedAttempts}");
                            return;
                        }

                        var list = edges.Select(edge => edge["node"]?.ToObject<InstaComment>())
                            .Where(post => post is not null)
                            .ToList();

                        if (list.Count > 0)
                        {
                            successAttempts++;
                            failedAttempts = 0;
                            errors.Clear();
                            collection.AddRange(list);
                            SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
                        }
                        else
                        {
                            failedAttempts++;
                            errors.Add("Required node edges not found");
                        }
                    }
                    else
                    {
                        var root = JObject.Parse(content);
                        if (root is null)
                        {
                            failedAttempts++;
                            SendTaskProgress(taskParams, $"Unable to get root object, failed {failedAttempts}");
                            return;
                        }

                        hasNextPage = root["data"]?[responseFilterValue]?["page_info"]?["has_next_page"].Value<bool>() ?? false;
                        var edges = (JArray)root["data"]?[responseFilterValue]?["edges"];
                        if (edges == null)
                        {
                            failedAttempts++;
                            SendTaskProgress(taskParams, $"Unable to get edges, failed {failedAttempts}");
                            return;
                        }

                        var list = edges.Select(edge => edge["node"]?.ToObject<InstaComment>())
                            .Where(post => post is not null)
                            .ToList();

                        if (list.Count > 0)
                        {
                            successAttempts++;
                            failedAttempts = 0;
                            errors.Clear();
                            collection.AddRange(list);
                            SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
                        }
                        else
                        {
                            failedAttempts++;
                            errors.Add("Required node edges not found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    failedAttempts++;
                    errors = [.. errors, .. ex.GetAllInnerMessages()];
                }
            }

            try
            {
                taskParams.WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                taskParams.WebView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;

                NavigateOrReload(taskParams.WebView, requestUrl);

                while (!taskParams.CancellationToken.IsCancellationRequested)
                {
                    WebViewHelper.ScrollDownAllDivOnPage(taskParams.WebView);

                    if (!hasNextPage) break;
                    if (taskParams.RecordsCount != 0 && collection.Count >= taskParams.RecordsCount) break;
                    if (failedAttempts == taskParams.FailedAttempts) break;
                    if (successAttempts > 0 && successAttempts % taskParams.LoopBreakAttempts == 0)
                    {
                        var wait = random.Next(15, 60);
                        SendTaskProgress(taskParams, $"Break loop wait for {wait} seconds...", true, TimeSpan.FromSeconds(wait));
                        await Task.Delay(TimeSpan.FromSeconds(wait), taskParams.CancellationToken.Token);
                        continue;
                    }

                    var timeWait = random.Next(taskParams.MinWait, taskParams.MaxWait);
                    SendTaskProgress(taskParams, $"Wait for {timeWait} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(timeWait), taskParams.CancellationToken.Token);
                }

                collection.ForEach(x => x.post_short_code = postShortCode);

                return new InstaResult<List<InstaComment>>()
                {
                    Status = true,
                    Content = collection,
                };
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaComment>>()
                {
                    Status = collection.Count > 0,
                    Content = collection,
                    Errors = ex.GetAllInnerMessages()
                };
            }
            finally
            {
                taskParams.WebView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
                taskParams.WebView.CoreWebView2.WebResourceResponseReceived -= OnWebResourceResponseReceived;
            }
        }


        public async Task<InstaResult<List<string>>> MonitorFollowRequest(InstaBulkTaskParams<List<string>> taskParams)
        {
            var requestFilter = "graphql/query";
            var requestFilterValue = "xdt_api__v1__friendships__create__target_user_id";
            var tcs = new TaskCompletionSource<InstaResult<List<string>>>();
            var collection = new List<string>();

            bool IsValidRequest(CoreWebView2WebResourceRequest Request)
            {
                var check1 = Request.Uri.Contains(requestFilter);
                var check2 = Request.Headers.Any(x => x.Key.Equals("X-Root-Field-Name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals(requestFilterValue, StringComparison.CurrentCultureIgnoreCase));
                var check3 = Request.Headers.Any(x => x.Key.Equals("x-fb-friendly-name", StringComparison.CurrentCultureIgnoreCase) && x.Value.Equals("usePolarisToggleFollowUserFollowMutation", StringComparison.CurrentCultureIgnoreCase));
                return check1 && check2 && check3;
            }

            async void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
            {
                if (!IsValidRequest(e.Request)) return;

                using var reader = new StreamReader(e.Request.Content);
                var postData = await reader.ReadToEndAsync();
                var postDataDict = postData.Split('&').Select(x => x.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);
                var decodedStr = WebUtility.UrlDecode(postDataDict["variables"]);
                var jsonObject = JObject.Parse(decodedStr);
                var user_id = jsonObject["target_user_id"]?.ToString();
                if (string.IsNullOrEmpty(user_id)) return;

                collection.Add(user_id);
                SendTaskProgress(taskParams, user_id);
            }

            try
            {
                taskParams.WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                taskParams.WebView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;

                taskParams.CancellationToken.Token.Register(() => tcs.TrySetResult(new InstaResult<List<string>>()
                {
                    Status = false,
                    Content = collection,
                    Errors = ["Task canceled by user"]
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<List<string>>
                {
                    Status = false,
                    Content = collection,
                    Errors = ex.GetAllInnerMessages(),
                };
            }
            finally
            {
                taskParams.WebView.CoreWebView2.WebResourceRequested -= OnWebResourceRequested;
            }
        }

        public async Task<string> GetUsernameByUserPk(string userPk)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 Instagram 12.0.0.16.90 (iPhone9,4; iOS 10_3_3; en_US; en-US; scale=2.61; gamut=wide; 1080x1920)");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            try
            {
                var response = await client.GetAsync($"https://i.instagram.com/api/v1/users/{userPk}/info/");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JObject.Parse(json);
                var username = doc?["user"]?["username"].ToString();
                return username;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetUserPkByUsername(string username)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 Instagram 12.0.0.16.90 (iPhone9,4; iOS 10_3_3; en_US; en-US; scale=2.61; gamut=wide; 1080x1920)");

            try
            {
                var response = await client.GetAsync($"https://www.instagram.com/web/search/topsearch/?query={username}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JObject.Parse(json);
                var userPk = doc?["users"]?[0]?["user"]?["pk"]?.ToString();
                return userPk;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InstaResult<string>> GetUserPkByUsername(WebView2 webview, string username)
        {
            var scriptName = "AjaxGetUserPk.js";

            async Task<string> CallAjaxAsync()
            {
                var requestId = Guid.NewGuid().ToString();
                var script = $"fetchUserPk('{username}')";

                var (status, result) = await WebViewHelper.ExecuteScriptForResult(webview, script);

                var root = JObject.Parse(result);
                var user_pk = root?["data"]?["users"]?[0]?["user"]?["pk"]?.ToString();
                return user_pk;
            }

            try
            {
                var (status, scriptText) = GetScriptFromFile(scriptName);
                if (!status)
                {
                    return new()
                    {
                        Status = false,
                        Content = null,
                        Errors = [$"{scriptName} not found"]
                    };
                }

                await webview.ExecuteScriptAsync(scriptText);
                var userPk = await CallAjaxAsync();

                return new()
                {
                    Status = !string.IsNullOrEmpty(userPk),
                    Content = userPk,
                    Errors = string.IsNullOrEmpty(userPk) ? [$"User pk not found for username {username}"] : null,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Status = false,
                    Content = null,
                    Errors = ex.GetAllInnerMessages()
                };
            }
        }
    }
}
