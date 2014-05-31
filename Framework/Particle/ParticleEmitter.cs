using System.Collections.Generic;

using MG.Framework.Numerics;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public abstract class ParticleEmitter
	{
		protected ParticleData particleData;
		protected ParticleDefinition particleDefinition;

		public ParticleEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
		{
			this.particleData = particleData;
			this.particleDefinition = particleDefinition;
		}

		public abstract void Update(Time time);
		public abstract int Emit();
		public abstract void Clear();
		public abstract void Reload();
	}

	public abstract class BasicParticleEmitter : ParticleEmitter
	{
		private ParticleDefinition.Parameter paramLife;
		private ParticleDefinition.Parameter paramSpawnRate;
		private ParticleDefinition.Parameter paramOffsetX;
		private ParticleDefinition.Parameter paramOffsetY;
		private List<Vector2> particlePosition;
		private List<Vector2> particleVelocity;
		private List<float> particleLife;
		private List<float> particleAge;

		private float particleSpawnAccumulator;

		public BasicParticleEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{
			particlePosition = particleData.Get<Vector2>("Position");
			particleVelocity = particleData.Get<Vector2>("Velocity");
			particleLife = particleData.Get<float>("Life");
			particleAge = particleData.Get<float>("Age");
		}

		public override void Reload()
		{
			paramLife = particleDefinition.Parameters["Life"];
			paramSpawnRate = particleDefinition.Parameters["SpawnRate"];
			paramOffsetX = particleDefinition.Parameters["OffsetX"];
			paramOffsetY = particleDefinition.Parameters["OffsetY"];
		}

		public override void Clear()
		{
			particleSpawnAccumulator = 0;
		}

		public override void Update(Time time)
		{
			particleSpawnAccumulator += time.ElapsedSeconds;

			var secondsPerParticle = 1.0f / ParticleHelpers.GetFloat(paramSpawnRate);
			while (particleSpawnAccumulator >= secondsPerParticle)
			{
				Emit();
				particleSpawnAccumulator -= secondsPerParticle;
			}
		}

		protected int EmitInternal(Vector2 position, Vector2 velocity, float life)
		{
			if (particleData.ActiveParticles + 1 >= particleData.MaxParticles) particleData.Resize();
			var index = particleData.ActiveParticles;
			particleData.ActiveParticles++;

			particlePosition[index] = position + new Vector2(ParticleHelpers.GetFloat(paramOffsetX), ParticleHelpers.GetFloat(paramOffsetY));
			particleVelocity[index] = velocity;
			particleAge[index] = 0;
			particleLife[index] = ParticleHelpers.GetFloat(paramLife);
			return index;
		}
	}

	public class PointEmitter : BasicParticleEmitter
	{
		public Vector2 Point;

		public PointEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{

		}

		public override int Emit()
		{
			return EmitInternal(Point, MathTools.Random().RandomDirection() * 40, 0);
		}
	}
}
