using MG.Framework.Utility;

namespace MG.Framework
{
	public static class Framework
	{
		private static bool initialized = false;

		/// <summary>
		/// Initialize the framework.
		/// </summary>
		public static void Initialize()
		{
			if (initialized) return;
			initialized = true;

			Log.Initialize("Logs");
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
