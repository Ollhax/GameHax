using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	static class ParticleHelpers
	{
		public static float GetFloat(ParticleDefinition.Parameter parameter)
		{
			float r = parameter.Random != null ? MathTools.Random().NextFloat(parameter.Random.Get<float>()) : 0;
			
			return parameter.Value.Get<float>() + r;
		}
	}
}
