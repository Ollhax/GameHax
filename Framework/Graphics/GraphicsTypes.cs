namespace MG.Framework.Graphics
{
	/// <summary>
	/// Blending mode for renderers.
	/// </summary>
	public enum BlendMode
	{
		BlendmodeOpaque, // Override pixels without using alpha
		BlendmodeAlpha, // Standard premultiplied alpha mode
		BlendmodeNonPremultiplied, // Multiply color values using alpha values
		BlendmodeAdditive // Add color values without using alpha
	};
}
