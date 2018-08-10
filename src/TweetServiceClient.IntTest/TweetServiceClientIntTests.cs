namespace TweetServiceClient.IntTest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Xunit;
    using TweetMocked = UnitTest.TweetServiceClientUnitTests;

    public class TweetServiceClientIntTests
    {
        private TweetServices tweetServiceClient;

        /// <summary>
        /// Gets the tweets async when large amount of tweets returns correct expected tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenTweetReturnsLargeAmount_ReturnsCorrectTweets()
        {
            // Arrange.
            var startDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());
            var endDateTime = new DateTimeOffset(2016, 1, 20, 0, 0, 0, new TimeSpan());

            var expectedTweetCount = 287;

            tweetServiceClient = new TweetServices(new HttpClient(), new TweetMocked.MockedLogger());


            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            Assert.NotNull(actualTweets);
            Assert.Equal(expectedTweetCount, actualTweets.Count);
            AssertUniqueTweets(actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when under limit returns correct tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenTweetReturnsSmallAmount_ReturnsCorrectTweets()
        {
            // Arrange.
            var startDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());
            var endDateTime = new DateTimeOffset(2016, 1, 4, 0, 0, 0, new TimeSpan());

            var expectedTweetCount = 45;

            tweetServiceClient = new TweetServices(new HttpClient(), new TweetMocked.MockedLogger());


            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            Assert.NotNull(actualTweets);
            Assert.Equal(expectedTweetCount, actualTweets.Count);
            AssertUniqueTweets(actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when under limit returns correct tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_UsingSameTime_ReturnsCorrectTweets()
        {
            // Arrange.
            var startDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());
            var endDateTime = new DateTimeOffset(2016, 1, 1, 0, 0, 0, new TimeSpan());

            var expectedTweetCount = 0;

            tweetServiceClient = new TweetServices(new HttpClient(), new TweetMocked.MockedLogger());


            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            Assert.NotNull(actualTweets);
            Assert.Equal(expectedTweetCount, actualTweets.Count);
            AssertUniqueTweets(actualTweets);
        }

        /// <summary>
        /// Assert that the tweets are unique.
        /// </summary>
        /// <param name="tweets">Tweets.</param>
        private void AssertUniqueTweets(IList<Tweet> tweets) 
        {
            var tweetSet = new HashSet<string>();

            foreach (var tweet in tweets) {
                Assert.DoesNotContain(tweet.id, tweetSet);
                tweetSet.Add(tweet.id);
            }
        }
    }
}
