namespace Tweets.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using TweetServiceClient;

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

            // Give user option to view tweets.
            Console.WriteLine("Pick an option: [1 - 3]");
            Console.WriteLine("1: Get all tweets from 2016 & 2017 UTC time");
            Console.WriteLine("2: Get all tweets and attempt save to a file (tweets.txt).");
            Console.WriteLine("3: Get all tweets and attempt save to a file along with a log (logs.txt).");
            int option = 0;
            Logger logger = null;

            // Check for the user input.
            if (Int32.TryParse(Console.ReadLine(), out option) && (option == 1 || option == 2 || option == 3)) 
            {
                logger = new Logger(false);
                if (option > 1) logger = new Logger(true);
                PrintTweets(logger, option);
            }
            else {
                Console.WriteLine("Invalid input. Program ended.");
            }
        }

        private static void PrintTweets(Logger logger, int option)
        {
            Console.WriteLine("Getting tweet identifier from 2016 & 2017 UTC.");

            var startDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());
            var endDateTime = new DateTimeOffset(2018, 1, 1, 0, 0, 0, new TimeSpan());
            endDateTime.AddMilliseconds(-1);

            var tweetServiceClient = new TweetServices(new HttpClient(), logger);
            IList<Tweet> tweets = tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime).Result;

            Console.WriteLine(string.Format("Found {0} tweets", tweets.Count));

            StreamWriter writetext = null;
            if (option > 1)
            {
                writetext = new StreamWriter("tweets.txt");
                writetext.AutoFlush = true;
                Console.SetOut(writetext);
            }
            foreach (var tweet in tweets)
            {
                Console.WriteLine(tweet);
                Console.WriteLine();
            }

            if (writetext != null) 
            {
                writetext.Close();
            }

            if (option == 3)
            {
                using (writetext = new StreamWriter("logs.txt"))
                {
                    writetext.WriteLine(logger.getAllLoggedMessage());
                }
            }
        }
    }
}
