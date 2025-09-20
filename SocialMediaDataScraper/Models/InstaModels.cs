#nullable disable

using LiteDB;
using Microsoft.Web.WebView2.WinForms;

namespace SocialMediaDataScraper.Models
{
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
        public ObjectId taskId { get; set; }

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
        public ObjectId taskId { get; set; }

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
        public ObjectId taskId { get; set; }

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
        public ObjectId taskId { get; set; }


        public string pk { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
        public string follower_username { get; set; }
    }

    ////////////////////////////////////////////////

    public class InstaReel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public ObjectId taskId { get; set; }


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
        [BsonId]
        public ObjectId _id { get; set; }
        public ObjectId taskId { get; set; }


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
