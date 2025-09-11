#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        public async Task<InstaResult<List<InstaReel>>> TestLogin(WebView2 webView, string username, TimeSpan? waitInSeconds = null)
        {
            var requestFilter = "graphql/query";
            var requestUrl = $"https://www.instagram.com/{username}";
            var responseFilterKey = "X-Root-Field-Name";
            var responseFilterValue = "xdt_api__v1__feed__reels_tray";
            var tcs = new TaskCompletionSource<InstaResult<List<InstaReel>>>();
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
                        tcs.TrySetResult(new InstaResult<List<InstaReel>> { Status = false, Errors = errors });
                        return;
                    }

                    var feed = rootObject?["data"]?[responseFilterValue]?["tray"]?.ToObject<List<InstaReel>>();
                    if (feed == null)
                    {
                        tcs.TrySetResult(new InstaResult<List<InstaReel>> { Status = false, Errors = ["Required node not found"] });
                        return;
                    }

                    isLogin = true;

                    tcs.SetResult(new InstaResult<List<InstaReel>>()
                    {
                        Status = true,
                        Content = feed,
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

                cts.Token.Register(() => tcs.TrySetResult(new InstaResult<List<InstaReel>>()
                {
                    Status = false,
                    Content = null,
                    Errors = [.. errors, "Request timed out"],
                }));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return new InstaResult<List<InstaReel>>
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
            var hasMore = false;
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
                SendTaskProgress(taskParams, $"{list.Count}/{collection.Count}/{taskParams.RecordsCount} records collected, attempt {successAttempts}");
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

                    if (hasMore == false) break;
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

        [BsonRef(nameof(DS_BrowserTask))]
        public DS_BrowserTask task { get;set;}

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
        public string username { get; set; }
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
        [BsonId]
        public ObjectId _id { get; set; }
        public string post_short_code { get; set; }
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
        public TimeSpan BreakLoopWait { get; set; }
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
