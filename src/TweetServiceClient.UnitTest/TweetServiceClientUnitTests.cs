namespace TweetServiceClient.UnitTest
{
    using System;
    using Xunit;
    using Moq;
    using TweetServiceClient;
    using System.Collections.Generic;
    using System.Net.Http;
    using Moq.Protected;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Threading;
    using System.Net;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions.Internal;

    public class TweetServiceClientUnitTests
    {
        private TweetServices tweetServiceClient;

        /// <summary>
        /// Gets the tweets when over 100 limit calls next get tweets async.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenOverLimit_CallsNextGetTweetsAsync()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = new List<List<Tweet>>();
            mockedTweets.Add(getTweets("first", 100));
            mockedTweets.Add(getTweets("second", 50));
            expectedTweets.AddRange(mockedTweets[0]);
            expectedTweets.AddRange(mockedTweets[1]);

            // Mock the response from http client.
            var responseMessage1 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[0]);
            var responseMessage2 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[1]);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage1, responseMessage2);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());


            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            AssertTweets(expectedTweets, actualTweets);
        }

        /// <summary>
        /// Gets the tweets when over limit with contains no duplicate tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenOverLimitWithSameTimeStamp_ContainsNoDuplicateTweets()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = new List<List<Tweet>>();
            mockedTweets.Add(getTweets("first", 100));
            mockedTweets.Add(getTweets("second", 50));
            expectedTweets.AddRange(mockedTweets[0]);

            // Make the first tweet of second api equal to previous call.
            mockedTweets[1][0] = new Tweet()
            {
                id = mockedTweets[0][99].id,
                stamp = mockedTweets[0][99].stamp,
                text = mockedTweets[0][99].text,
            };

            // Skip the first one.
            expectedTweets.AddRange(mockedTweets[1].GetRange(1, 49));


            // Mock the response from http client.
            var responseMessage1 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[0]);
            var responseMessage2 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[1]);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage1, responseMessage2);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());


            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            AssertTweets(expectedTweets, actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when under limit returns correct tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenUnderLimit_ReturnsCorrectTweets()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = getTweets("first", 50);
            expectedTweets.AddRange(mockedTweets);

            // Mock the response from http client.
            var responseMessage = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());

            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            AssertTweets(expectedTweets, actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when empty returns correct empty tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenEmptyTweets_ReturnsEmptyTweets()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = getTweets("first", 0);
            expectedTweets.AddRange(mockedTweets);

            // Mock the response from http client.
            var responseMessage = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());

            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            AssertTweets(expectedTweets, actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when count is exact max count, returns correct tweets.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenExact100Tweets_ReturnCorrectTweets()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = new List<List<Tweet>>();
            mockedTweets.Add(getTweets("first", 100));
            mockedTweets.Add(getTweets("second", 0));
            expectedTweets.AddRange(mockedTweets[0]);
            expectedTweets.AddRange(mockedTweets[1]);

            // Mock the response from http client.
            var responseMessage1 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[0]);
            var responseMessage2 = getHttpResponseMessage(HttpStatusCode.OK, mockedTweets[1]);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage1, responseMessage2);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());

            // Act.
            IList<Tweet> actualTweets = await tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);

            // Assert.
            AssertTweets(expectedTweets, actualTweets);
        }

        /// <summary>
        /// Gets the tweets async when error response, throws exception.
        /// </summary>
        [Fact]
        public async void GetTweetsAsync_WhenErrorResponse_ThrowsException()
        {
            // Arrange.
            var startDateTime = DateTimeOffset.UtcNow;
            var endDateTime = DateTimeOffset.UtcNow;
            endDateTime = endDateTime.AddDays(1);
            var expectedTweets = new List<Tweet>();
            var mockedTweets = getTweets("first", 1);
            expectedTweets.AddRange(mockedTweets);

            // Mock the response from http client.
            var responseMessage = getHttpResponseMessage(HttpStatusCode.NotFound, mockedTweets);
            var mockedMessageHandler = getMockedMessageHandler(responseMessage);
            tweetServiceClient = new TweetServices(new HttpClient(mockedMessageHandler.Object), new MockedLogger());

            // Act.
            // Assert.
            await Assert.ThrowsAnyAsync<Exception>(() =>
            {
                return tweetServiceClient.GetTweetsAsync(startDateTime, endDateTime);
            });

        }

        /// <summary>
        /// Gets the tweets based on given count.
        /// </summary>
        /// <param name="id">The tweet identifier.</param>
        /// <param name="count">The number of tweets requested.</param>
        /// <returns>The tweets.</returns>
        private List<Tweet> getTweets(string id, int count)
        {
            List<Tweet> tweets = new List<Tweet>();

            for (int i = 0; i < count && i < 100; i++)
            {
                tweets.Add(new Tweet()
                {
                    id = id + i.ToString(),
                    stamp = DateTimeOffset.UtcNow.AddMilliseconds(i),
                    text = i.ToString()
                });
            }

            return tweets;
        }

        /// <summary>
        /// Asserts the tweets.
        /// </summary>
        /// <param name="expectedTweets">Expected tweets.</param>
        /// <param name="actualTweets">Actual tweets.</param>
        private void AssertTweets(IList<Tweet> expectedTweets, IList<Tweet> actualTweets)
        {
            Assert.NotNull(actualTweets);
            Assert.Equal(expectedTweets.Count, actualTweets.Count);
            for (int i = 0; i < actualTweets.Count && expectedTweets.Count == actualTweets.Count; i++)
            {
                Assert.Equal(expectedTweets[i].id, actualTweets[i].id);
                Assert.Equal(expectedTweets[i].text, actualTweets[i].text);
            }
        }

        /// <summary>
        /// Gets http response message.
        /// </summary>
        /// <param name="code">Http status Code.</param>
        /// <param name="tweets">Mocked Tweets.</param>
        /// <returns>The http response message.</returns>
        private HttpResponseMessage getHttpResponseMessage(HttpStatusCode code, List<Tweet> tweets)
        {
            return new HttpResponseMessage()
            {
                StatusCode = code,
                Content = new StringContent(JsonConvert.SerializeObject(tweets))
            };

        }

        /// <summary>
        /// Gets the mocked message handler. from the given response message. 
        /// </summary>
        /// <param name="responseMessage1">Response message1.</param>
        /// <param name="responseMessage2">Response message2.</param>
        /// <returns>The mocked message handler.</returns>
        private Mock<HttpMessageHandler> getMockedMessageHandler(HttpResponseMessage responseMessage1, HttpResponseMessage responseMessage2)
        {
            Mock<HttpMessageHandler> mockedMessageHandler = new Mock<HttpMessageHandler>();
            mockedMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(responseMessage1))
                .Returns(Task.FromResult(responseMessage2));

            return mockedMessageHandler;
        }

        /// <summary>
        /// Gets the mocked message handler. from the given response message. 
        /// </summary>
        /// <param name="responseMessage">Response message.</param>
        /// <returns>The mocked message handler.</returns>
        private Mock<HttpMessageHandler> getMockedMessageHandler(HttpResponseMessage responseMessage)
        {
            Mock<HttpMessageHandler> mockedMessageHandler = new Mock<HttpMessageHandler>();
            mockedMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(responseMessage));

            return mockedMessageHandler;
        }

        public class MockedLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return NullScope.Instance;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                
            }
        }
    }
}
