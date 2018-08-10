namespace TweetServiceClient
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains event types for tweet service logging.
    /// </summary>
    public static class TweetServiceLogEvent
    {
        public static EventId GetTweetAsyncStarted = new EventId(0);
        public static EventId GetTweetAsyncEnded = new EventId(1);
        public static EventId GetTweetAsyncException = new EventId(2);
        public static EventId GetRestOfTweets = new EventId(3);

    }
}