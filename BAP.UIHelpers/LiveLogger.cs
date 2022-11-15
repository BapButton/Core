using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAP.UIHelpers
{
	public static class LiveLogger
	{
		private static int CurrentId { get; set; } = 0;
		private static FixedSizedQueue<(int logNumber, string source, LogLevel level, string message)> Queue { get; set; } = new(50);
		public static event EventHandler<LogGeneratedEventArgs> LogGenerated = delegate { };
		public static void RecordNewLogMessage(string source, NLog.LogLevel level, string message)
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
			Queue.Enqueue((CurrentId, source, logLevel, message));
			LogGenerated.Invoke(null, new LogGeneratedEventArgs(CurrentId, source, logLevel, message));
		}
		public static List<(int logNumber, string source, LogLevel level, string message)> GetCurrentLogs()
		{
			return Queue.ToList();
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

	public class FixedSizedQueue<T> : ConcurrentQueue<T>
	{
		private readonly object syncObject = new object();

		public int Size { get; private set; }

		public FixedSizedQueue(int size)
		{
			Size = size;
		}

		public new void Enqueue(T obj)
		{
			base.Enqueue(obj);
			lock (syncObject)
			{
				while (Count > Size)
				{
					TryDequeue(out _);
				}
			}
		}
	}
}
