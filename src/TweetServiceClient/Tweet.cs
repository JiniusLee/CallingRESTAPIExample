namespace TweetServiceClient
{
    using System;

    /// <summary>
    /// Contains the data for the tweet.
    /// </summary>
    public class Tweet
    {
        public string id { get; set; }

        public DateTimeOffset stamp { get; set; }

        public string text { get; set; }

        public override string ToString() {
            
            return string.Format("[{0} - {1}] {2}", id, stamp.ToString("o"), text);
        }
    }
}