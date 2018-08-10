namespace Tweets.Console
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions.Internal;

    public class Logger : ILogger
    {
        private bool isLogEnabled;
        private string loggedMessage;

        public Logger(bool isLogEnabled = false) {
            this.isLogEnabled = isLogEnabled;
            this.loggedMessage = "";
        }
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
            if (isLogEnabled) {
                loggedMessage += string.Format("Log level {0}, event id: {1} - {2} \n\n", logLevel, eventId, formatter?.Invoke(state, exception));   
            }
        }

        public string getAllLoggedMessage() {
            return loggedMessage;
        }
    }
}
