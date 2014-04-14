using System;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// Generic alignment type.
	/// </summary>
	public enum AlignmentType
	{
		Center, North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
	}

	/// <summary>
	/// Helpers for the alignment type.
	/// </summary>
	public static class AlignmentTools
	{
		public static bool FromString(string s, out AlignmentType alignment)
		{
			if (string.Equals(s, "c", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.Center; return true; }
			if (string.Equals(s, "n", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.North; return true; }
			if (string.Equals(s, "ne", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.NorthEast; return true; }
			if (string.Equals(s, "e", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.East; return true; }
			if (string.Equals(s, "se", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.SouthEast; return true; }
			if (string.Equals(s, "s", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.South; return true; }
			if (string.Equals(s, "sw", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.SouthWest; return true; }
			if (string.Equals(s, "w", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.West; return true; }
			if (string.Equals(s, "nw", StringComparison.InvariantCultureIgnoreCase)) { alignment = AlignmentType.NorthWest; return true; }
			
			alignment = AlignmentType.Center;
			return false;
		}
	}
}
