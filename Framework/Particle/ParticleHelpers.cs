using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	public static class ParticleHelpers
	{
		public static float ToRadians(float particleAngle)
		{
			return MathTools.ToRadians(particleAngle - 90);
		}
	}
}
