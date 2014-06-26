using System.Threading;

using MG.Framework.Utility;
using System;
using System.IO;

namespace MG.Framework
{
	public static class Framework
	{
		public static FilePath SaveDataFolder { get; private set; }

		private static bool initialized = false;

		/// <summary>
		/// Initialize the framework.
		/// </summary>
		public static void Initialize(string mainThreadName, string saveDataFolder)
		{
			if (initialized) return;
			initialized = true;

			if (Thread.CurrentThread.Name == null)
			{
				Thread.CurrentThread.Name = mainThreadName;
			}

			if (Platform.IsWindows)
			{
				SaveDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), saveDataFolder);
			}
			else
			{
				SaveDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", saveDataFolder);
			}

			if (!Directory.Exists(SaveDataFolder))
			{
				Directory.CreateDirectory(SaveDataFolder);
			}

			ExceptionHandler.Initialize();
			Log.Initialize(SaveDataFolder);
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
