namespace Tweets.Console
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using TweetServiceClient;
    using TweetMocked = TweetServiceClient.UnitTest.TweetServiceClientUnitTests;

    /// <summary>
    /// Basic program to get 2016 & 2017 tweets.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("This will be calling the TweetServces to get the tweets");
            Console.WriteLine("This would later can also be used by by UI given more time by using this library.");

            Console.WriteLine("Getting tweet identifier from 2016 & 2017 UTC.");

            var startDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());
            var endDateTime = new DateTimeOffset(2018, 1, 1, 0, 0, 0, new TimeSpan());
            endDateTime.AddMilliseconds(-1);
            var tweetServiceClient = new TweetServices(new HttpClient(), new TweetMocked.MockedLogger());
            IList<Tweet> tweets = tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime).Result;
            PrintTweet(tweets);

            Console.WriteLine("Assumptions made: Getting 2016 - 2017 UTC Date. - Should have clarified.");
        }

        private static void PrintTweet(IList<Tweet>  tweets) {
            foreach (var tweet in tweets) {
                Console.WriteLine(tweet);
                Console.WriteLine();
            }

            Console.WriteLine(string.Format("Found {0} tweets", tweets.Count));
        }
    }
}
