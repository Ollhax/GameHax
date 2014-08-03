using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	public class RandomInt
	{
		private readonly ParticleDefinition definition;
		private readonly string parameterName;

		private int parameterValue;
		private int randomValue;

		public int BaseValue { get { return parameterValue; } }

		public bool IsZero { get { return parameterValue == 0 && randomValue == 0; } }

		public RandomInt(ParticleDefinition definition, string parameterName)
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
				parameterValue = parameter.Value.Get<int>();
				randomValue = 0;
				
				ParticleDefinition.Parameter parameterRandom;
				if (parameter.Parameters.TryGetValue("Random", out parameterRandom))
				{
					randomValue = parameterRandom.Value.Get<int>();
				}
			}
		}

		public int Get(float emitterLifeFraction, float particleLifeFraction)
		{
			var v = parameterValue;

			if (randomValue != 0)
			{
				v += MathTools.Random().Next(-randomValue, randomValue);
			}
			
			return v;
		}
	}
}
