using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	// TODO: Move to definition-level? RandomFloats do not change per instance
	class RandomFloat
	{
		private readonly ParticleDefinition definition;
		private readonly string parameterName;

		private float parameterValue;
		private float randomValue;
		private ComplexCurve graphEmitter;
		private ComplexCurve graphParticle;

		public RandomFloat(ParticleDefinition definition, string parameterName)
		{
			this.definition = definition;
			this.parameterName = parameterName;
			Reload();
		}

		public void Reload()
		{
			ParticleDefinition.Parameter parameter;
			if (definition.Parameters.TryGetValue(parameterName, out parameter))
			{
				parameterValue = parameter.Value.Get<float>();
				graphEmitter = null;
				graphParticle = null;
				randomValue = 0;
				
				ParticleDefinition.Parameter parameterRandom;
				if (parameter.Parameters.TryGetValue("Random", out parameterRandom))
				{
					randomValue = parameterRandom.Value.Get<float>();
				}

				ParticleDefinition.Parameter parameterGraph;
				if (parameter.Parameters.TryGetValue("GraphEmitter", out parameterGraph))
				{
					var s = parameterGraph.Value.Get<string>();
					if (!string.IsNullOrEmpty(s))
					{
						graphEmitter = ComplexCurve.FromInvariantString(parameterGraph.Value.Get<string>());
					}
				}

				if (parameter.Parameters.TryGetValue("GraphParticle", out parameterGraph))
				{
					var s = parameterGraph.Value.Get<string>();
					if (!string.IsNullOrEmpty(s))
					{
						graphParticle = ComplexCurve.FromInvariantString(parameterGraph.Value.Get<string>());
					}
				}
			}
		}

		public float Get(float emitterLifeFraction, float particleLifeFraction)
		{
			var v = parameterValue;

			if (randomValue != 0)
			{
				v += MathTools.Random().NextFloat(-randomValue, randomValue);

			}

			if (graphEmitter != null)
			{
				v *= graphEmitter.Evaluate(emitterLifeFraction);
			}

			if (graphParticle != null)
			{
				v *= graphParticle.Evaluate(particleLifeFraction);
			}

			return v;
		}
	}
}
