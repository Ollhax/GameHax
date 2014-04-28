using System.Threading;

using MG.Framework.Utility;

namespace MG.Framework
{
	public static class Framework
	{
		private static bool initialized = false;

		/// <summary>
		/// Initialize the framework.
		/// </summary>
		public static void Initialize(string mainThreadName, string logPath)
		{
			if (initialized) return;
			initialized = true;

			if (Thread.CurrentThread.Name == null)
			{
				Thread.CurrentThread.Name = mainThreadName;
			}

			Log.Initialize(logPath);
		}

		/// <summary>
		/// Deinitialize the framework.
		/// </summary>
		public static void Deinitialize()
		{
			if (!initialized) return;
			initialized = false;

			Log.Deinitialize();
		}
	}
}
