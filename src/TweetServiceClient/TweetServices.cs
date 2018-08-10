namespace TweetServiceClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Retrieves the tweets by calling the external API. 
    /// </summary>
    public class TweetServices

    {
        private readonly int MAXCOUNT = 100;
		private static HttpClient client;
        private ILogger logger;

        public TweetServices(HttpClient httpClient, ILogger logger)
        {
            client = httpClient;
            this.logger = logger;
            Uri serviceUri = new Uri("https://badapi.iqvia.io/");
            client.BaseAddress = serviceUri;
        }

        /// <summary>
        /// Gets the tweets asynchronously.
        /// </summary>
        /// <param name="startDateTime">Start date time.</param>
        /// <param name="endDateTime">End date time.</param>
        /// <returns>The task that contains set of tweets.</returns>
        public async Task<IList<Tweet>> GetTweetsAsync(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            var tweets = new List<Tweet>();
            // Get the first 100 or less tweets for given date.
            IList<Tweet> partialTweets = await GetPartialTweetsAsync(startDateTime, endDateTime);

            if (partialTweets.Count > 0)
            {
                tweets.AddRange(partialTweets);

                // We should use set because there may be two tweets at exact same time.
                // For example, if there is a tweet A and tweet B at same time stamp,
                // If the first getTweets get all the tweet up to tweet A, then 
                // we CANNOT just get tweets EXCLUDING the endDateTime because this
                // will result in not being able to get tweet B.
                // In order to get tweet B, the next getTweets must query from the last 
                // pulled tweet date. However, this causes a NEW problem,
                // the getTweets will now also get tweet A along with tweet B.
                // Therefore, we have to save ALL tweet id for the last time stamp
                // since those will be DUPLICATED next pull.
                var lastTweetSet = new HashSet<string>();
                DateTimeOffset partialTweetLastDate = partialTweets.Last().stamp;
                lastTweetSet.UnionWith(partialTweets.Where(x => x.stamp == partialTweetLastDate).Select(x => x.id));

                // If the result tweet is max capacity, that means there may be more tweets within those time frame.
                while (partialTweets.Count == MAXCOUNT)
                {
                    this.logger.LogInformation(TweetServiceLogEvent.GetRestOfTweets, "Getting rest of tweets");
                    // Set the new start time from the last date time offset. Exclude the duplicated tweets.
                    DateTimeOffset newStartTime = partialTweets.Last().stamp;
                    partialTweets = await GetPartialTweetsAsync(newStartTime, endDateTime);
                    if (partialTweets.Count == 0) break;
                    tweets.AddRange(partialTweets.Where(x => !lastTweetSet.Contains(x.id)));

                    // Set the last tweets set.
                    lastTweetSet.Clear();
                    partialTweetLastDate = partialTweets.Last().stamp;
                    lastTweetSet.UnionWith(partialTweets.Where(x => x.stamp == partialTweetLastDate).Select(x => x.id));
                }
            }

            return tweets;
        }

        /// <summary>
        /// Gets the partial tweets asynchronously.
        /// </summary>
        /// <remarks>
        /// Gets only the partial tweets because the API only returns at most 100.
        /// </remarks>
        /// <param name="startDateTime">Start date time.</param>
        /// <param name="endDateTime">End date time.</param>
        /// <returns>The task that contains list of tweets.</returns>
        private async Task<IList<Tweet>> GetPartialTweetsAsync(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            var getTweetsUrl = string.Format("api/v1/Tweets?startDate={0}&endDate={1}", startDateTime.UtcDateTime.ToString("o"), endDateTime.UtcDateTime.ToString("o"));
            var partialTweets = new List<Tweet>();

            try
            {
                this.logger.LogInformation(TweetServiceLogEvent.GetTweetAsyncStarted, string.Format("Getting tweets from {0} - {1}", startDateTime.UtcDateTime, endDateTime.UtcDateTime));
                // From investigation, following assumptions are made.
                // 1. It returns the first 100 (or less) tweets.
                // 2. It returns in order by date time.
                // 3. There are no more than 100 of tweets with EXACT same datetime.
                // 4. There are no exact 100 of tweets with EXACT same datetime.

                // If assumption 1 is wrong: If it returns the last 100, the logic just have to be backward. 
                //                           If random, this would require tweet returned at a time is < 100 to make sure we didn't miss any.
                // If assumption 2 is wrong: I would just have to sort after retrieving it. 
                // If assumption 3 is wrong: It cannot be solved as no way to get the remaining tweets in the time frame.
                // If assumption 4 is wrong: It can be solved, just have to make sure next 'newstarttime' is 1 millisecond after.
                HttpResponseMessage responseMessage = await client.GetAsync(getTweetsUrl);
                responseMessage.EnsureSuccessStatusCode();
                string jsonResult = await responseMessage.Content.ReadAsStringAsync();

                partialTweets = JsonConvert.DeserializeObject<List<Tweet>>(jsonResult);

                this.logger.LogInformation(TweetServiceLogEvent.GetTweetAsyncEnded, string.Format(
                    "Successfully retrieved {0} tweets", partialTweets.Count));
                return partialTweets;
            }
            catch (Exception ex)
            {
                // The call did not result as expected.
                this.logger.LogError(TweetServiceLogEvent.GetTweetAsyncException, ex, "Failed to get tweets");
                throw;
            }
        }
    }
}
