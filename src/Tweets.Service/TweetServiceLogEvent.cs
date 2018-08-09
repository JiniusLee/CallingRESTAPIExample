namespace TweetServiceClient
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains event types for tweet service logging.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TweetServiceLogEvent
    {
        GetTweetAsyncStarted,
        GetTweetAsyncEnded,
        GetTweetAsyncException,
        GetRestOfTweets,
    }
}