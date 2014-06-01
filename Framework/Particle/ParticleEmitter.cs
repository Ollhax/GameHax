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
		private RandomFloat paramLife;
		private RandomFloat paramSpawnRate;
		private RandomFloat paramOffsetX;
		private RandomFloat paramOffsetY;
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
			paramLife = new RandomFloat(particleDefinition.Parameters["Life"]);
			paramSpawnRate = new RandomFloat(particleDefinition.Parameters["SpawnRate"]);
			paramOffsetX = new RandomFloat(particleDefinition.Parameters["OffsetX"]);
			paramOffsetY = new RandomFloat(particleDefinition.Parameters["OffsetY"]);
		}

		public override void Clear()
		{
			particleSpawnAccumulator = 0;
		}

		public override void Update(Time time)
		{
			particleSpawnAccumulator += time.ElapsedSeconds;

			var secondsPerParticle = 1.0f / paramSpawnRate;
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

			particlePosition[index] = position + new Vector2(paramOffsetX, paramOffsetY);
			particleVelocity[index] = velocity;
			particleAge[index] = 0;
			particleLife[index] = paramLife;
			return index;
		}
	}

	public class PointEmitter : BasicParticleEmitter
	{
		public Vector2 Point;
		
		private RandomFloat paramDirection;
		private RandomFloat paramRange;

		public PointEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{

		}

		public override void Reload()
		{
			base.Reload();

			paramDirection = new RandomFloat(particleDefinition.Parameters["Direction"]);
			paramRange = new RandomFloat(particleDefinition.Parameters["Range"]);
		}

		public override int Emit()
		{
			float range = paramRange;
			float direction = paramDirection + MathTools.Random().NextFloat(-range, range);

			return EmitInternal(Point, MathTools.FromAngle(MathTools.ToRadians(direction)) * 40, 0);
		}
	}
}
