using System;

namespace MG.Framework.Utility
{
	/// <summary>
	/// Holder of various time data necessary to update the game state.
	/// </summary>
	public struct Time
	{
		/// <summary>
		/// Elapsed seconds since last game frame.
		/// </summary>
		public float ElapsedSeconds;

		/// <summary>
		/// Total seconds since start of a specific level.
		/// </summary>
		public double TotalElapsedSeconds;
		
		/// <summary>
		/// Create a time holder.
		/// </summary>
		/// <param name="elapsedSeconds">Elapsed seconds since last game frame.</param>
		/// <param name="totalElapsedSeconds">Total seconds since start of a specific level.</param>
		public Time(float elapsedSeconds, double totalElapsedSeconds)
		{
			ElapsedSeconds = elapsedSeconds;
			TotalElapsedSeconds = totalElapsedSeconds;
		}
		
		/// <summary>
		/// Return a string version of this structure.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("Elapsed: {0}, Total: {1:0.00}", ElapsedSeconds, TotalElapsedSeconds);
		}
	}
}