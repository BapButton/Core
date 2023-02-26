using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAP.Helpers;

namespace BAP.WebCore
{
    public class DefaultLogProvider : ILogProvider
    {

        IPublisher<LogMessage> LogSender;
        public DefaultLogProvider(IPublisher<LogMessage> logMessageSender)
        {
            LogSender = logMessageSender;
        }

        private int CurrentId { get; set; } = 0;
        private FixedSizedQueue<LogMessage> Queue { get; set; } = new(100);

        public void RecordNewLogMessage(string source, NLog.LogLevel level, string message)
        {
            CurrentId++;
            LogLevel logLevel = LogLevel.Trace;
            if (level == NLog.LogLevel.Info)
            {
                logLevel = LogLevel.Information;
            }
            else if (level == NLog.LogLevel.Debug)
            {
                logLevel = LogLevel.Debug;
            }
            else if (level == NLog.LogLevel.Fatal)
            {
                logLevel = LogLevel.Error;
            }
            else if (level == NLog.LogLevel.Trace)
            {
                logLevel = LogLevel.Trace;
            }
            LogMessage newMessage = new LogMessage(CurrentId, source, logLevel, message);
            Queue.Enqueue(newMessage);
            LogSender.Publish(newMessage);
        }
        public List<LogMessage> GetCurrentLogs()
        {
            return Queue.ToList();
        }

        public List<LogMessage> GetLogs(LogLevel logLevel, int numberToFetch)
        {
            return Queue.Where(t => t.Level >= logLevel).Take(numberToFetch).ToList();
        }

        public Task<bool> InitializeAsync()
        {
            return Task.FromResult(true);
        }

        public void Dispose()
        {

        }
    }

    public class LogGeneratedEventArgs : EventArgs
    {
        public int MessageId { get; set; }
        public string Source { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public LogGeneratedEventArgs(int messageId, string source, LogLevel level, string message)
        {
            MessageId = messageId;
            Source = source;
            Level = level;
            Message = message;
        }
    }
}
