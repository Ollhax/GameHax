using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	class RandomFloat
	{
		private readonly ParticleDefinition.Parameter parameter;
		private readonly ParticleDefinition.Parameter random;

		public RandomFloat(ParticleDefinition.Parameter parameter)
		{
			this.parameter = parameter;
			parameter.Parameters.TryGetValue("Random", out random);
		}

		public static implicit operator float(RandomFloat p)
		{
			var v = p.parameter.Value.Get<float>();
			if (p.random != null)
			{
				var r = p.random.Value.Get<float>();
				v += MathTools.Random().NextFloat(-r, r);
			}

			return v;
		}
	}
}
