using System;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Flags for QuadBatch renderer.
	/// </summary>
	[Flags]
	public enum QuadEffects
	{
		None = 0,
		FlipHorizontally = 1,
		FlipVertically = 2,
		RoundPositions = 4,
	}
}
