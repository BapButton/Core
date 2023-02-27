﻿using Microsoft.Extensions.Logging;
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

        HashSet<string> BapRelatedPrefixes { get; set; } = new() { "BAP" };
        IPublisher<LogMessage> LogSender;
        public DefaultLogProvider(IPublisher<LogMessage> logMessageSender, LoadedAddonHolder loadedAddonHolder)
        {
            LogSender = logMessageSender;
            foreach (var item in loadedAddonHolder.AllAddonAssemblies)
            {
                string? name = item?.GetName()?.Name;
                if (name != null)
                {
                    BapRelatedPrefixes.Add(name);
                }
            }
            foreach (var item in loadedAddonHolder.AssembliesWithPages)
            {
                string? name = item?.GetName()?.Name;
                if (name != null)
                {
                    BapRelatedPrefixes.Add(name);
                }
            }
        }

        private int CurrentId { get; set; } = 0;
        private FixedSizedQueue<LogMessage> Queue { get; set; } = new(100);

        public void RecordNewLogMessage(string source, LogLevel logLevel, string message)
        {
            if (BapRelatedPrefixes.Any(t => t.StartsWith(source.Split('.')[0])))
            {
                CurrentId++;

                LogMessage newMessage = new LogMessage(CurrentId, source, logLevel, message);
                Queue.Enqueue(newMessage);
                LogSender.Publish(newMessage);
            }

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
