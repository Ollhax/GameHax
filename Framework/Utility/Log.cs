using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MG.Framework.Utility
{
	public enum LogTier
	{
		Frequent,
		Debug,
		Info,
		Warning,
		Error,
		Fatal
	};

	/// <summary>
	/// Generic log utility.
	/// </summary>
	public static class Log
	{
		private static FileStream logFile;
		private static TextWriter logWriter;
		private static BlockingCollection<LogEntry> entries = new BlockingCollection<LogEntry>();
		
		/// <summary>
		/// Max log size in bytes
		/// </summary>
		const int MaxLogSize = 1000000;

		/// <summary>
		/// Max log age in days
		/// </summary>
		const int MaxLogAge = 20;
		
		/// <summary>
		/// Initialize the log system.
		/// </summary>
		public static void Initialize(string logFilePath)
		{
			var path = Path.Combine(logFilePath, "Logs");

			// Creates the log directory
			Directory.CreateDirectory(path);

			// Remove old / too big logs
			string[] logs = Directory.GetFiles(path, "*txt");

			FileInfo logInfo;
			for (int i = 0; i < logs.Length; i++)
			{
				logInfo = new FileInfo(logs[i]);
				if (logInfo.Length >= MaxLogSize || logInfo.CreationTime < DateTime.Now.AddDays(-MaxLogAge))
					logInfo.Delete();
			}

			// Create/open log file
			try
			{
				logFile = File.Open(Path.Combine(path, "log " + DateTime.Now.Date.ToString("yyyy/MM/dd") + ".txt"), FileMode.Append);
				logWriter = new StreamWriter(logFile);
				logWriter.WriteLine("----------------------------------------------");
			}
			catch (IOException ioException)
			{
				// Ignore
			}
			
			// Start the consumer task
			Task.Factory.StartNew(WriteLog, TaskCreationOptions.LongRunning);
			
			Info("Starting log. Time: " + DateTime.Now.ToString());
		}
		
		/// <summary>
		/// Deinitialize the log system.
		/// </summary>
		public static void Deinitialize()
		{
			Info("Stopping log. Time: " + DateTime.Now.ToString());
			entries.CompleteAdding();

			if (logWriter != null)
			{
				logWriter.Dispose();
				logWriter = null;
			}

			if (logFile != null)
			{
				logFile.Dispose();
				logFile = null;
			}
		}
		
		/// <summary>
		/// Temporary print command for lazy typers.
		/// </summary>
		[Conditional("DEBUG")]
		public static void P(string text)
		{
			Info(text);
		}

		/// <summary>
		/// Asserts that a certain condition is true.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string text, int stackLevel = 1)
		{
			if (!condition)
			{
				PrintInternal(LogTier.Error, text, stackLevel);
			}
		}

		/// <summary>
		/// Print the specified text to frequent log.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Frequent(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Frequent, text, stackLevel);
		}

		/// <summary>
		/// Print the specified text to debug log.
		/// </summary>
		[Conditional("DEBUG")]
		public static void Debug(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Debug, text, stackLevel);
		}
		
		/// <summary>
		/// Print the specified text to info log.
		/// </summary>
		public static void Info(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Info, text, stackLevel);
		}

		/// <summary>
		/// Print the specified text to warning log.
		/// </summary>
		public static void Warning(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Warning, text, stackLevel);
		}
		
		/// <summary>
		/// Print the specified text to error log.
		/// </summary>
		public static void Error(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Error, text, stackLevel);
		}
		
		/// <summary>
		/// Print the specified text to fatal log.
		/// </summary>
		public static void Fatal(string text, int stackLevel = 1)
		{
			PrintInternal(LogTier.Fatal, text, stackLevel);
		}

		private struct LogEntry
		{
			public LogTier Tier;
			public string Text;
			public StackTrace StackTrace;
			public int StackLevel;
			public string ThreadName;
			public DateTime Timestamp;
		}
		
		private static void PrintInternal(LogTier logTier, string text, int stackLevel)
		{
			var entry = new LogEntry();
			entry.Timestamp = DateTime.Now;
			entry.Tier = logTier;
			entry.Text = text;
			entry.StackTrace = new StackTrace(1, true);
			entry.StackLevel = stackLevel;
			entry.ThreadName = Thread.CurrentThread.Name;
				
			entries.Add(entry);
			
			if (logTier == LogTier.Fatal)
			{
				throw new Exception("Fatal error: " + text);
			}
		}

		private static void WriteLog()
		{
			foreach (var p in entries.GetConsumingEnumerable())
			{
				string local = "";
				const int localLength = 25;
				const int systemLength = 10;
				const int levelLength = 8;

				StackFrame sf = p.StackTrace.GetFrame(p.StackLevel);
				string file = sf.GetFileName();
				if (file == null)
				{
					file = "";
				}

				local = PathHelper.GetFileNameWithoutPath(Path.GetFileNameWithoutExtension(file)) +
						"." + sf.GetMethod().Name;

				var localNumber = ":" + sf.GetFileLineNumber();

				if (local.Length > localLength - localNumber.Length)
				{
					local = local.Substring(0, localLength - localNumber.Length);
				}
				local += localNumber;

				string format = "{0}> {1,-" + (systemLength + 3) + "}{2,-" + (localLength + 1) + "}{3," + (levelLength + 1) + "}: {4}";

				string line = String.Format(format,
											p.Timestamp.ToString("HH:mm:ss.ffff"),
											"[" + p.ThreadName + "]",
											local,
											p.Tier,
											p.Text
					);

				Console.WriteLine(line);

				if (logWriter != null)
				{
					logWriter.WriteLine(p.Timestamp.ToString("HH:mm:ss") + "> " + line);
				}
			}
		}
	}
}