namespace MG.Framework.Numerics
{
	/// <summary>
	/// Tweening methods.
	/// </summary>
	public enum Tween
	{
		One,                // y = 1
		Zero,               // y = 0
		SmoothStep,         // y = x*x*(3 - 2*x)
		Linear,             // y = x
		Quadratic,          // y = x^2
		SmoothQuadratic,    // y = 1 - (x-1)^2
		InvSmoothStep,      // y = 1 - x*x*(3 - 2*x)
		InvLinear,          // y = 1 - x
		InvQuadratic,       // y = 1 - x^2
		InvSmoothQuadratic, // y = (x-1)^2
	};
}
