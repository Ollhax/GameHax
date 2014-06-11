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
		private float paramEmitterLife;
		private RandomFloat paramParticleLife;
		private RandomFloat paramSpawnRate;
		private RandomFloat paramOffsetX;
		private RandomFloat paramOffsetY;
		private List<Vector2> particlePosition;
		private List<Vector2> particleVelocity;
		private List<float> particleLife;
		private List<float> particleAge;

		private float emitterAge;
		private float particleSpawnAccumulator;

		public float EmitterLifeFractional { get { return paramEmitterLife > 0 ? emitterAge / paramEmitterLife : emitterAge; } }
		public bool Alive { get { return paramEmitterLife <= 0 || emitterAge < paramEmitterLife; } }

		public BasicParticleEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{
			particlePosition = particleData.Get<Vector2>("Position");
			particleVelocity = particleData.Get<Vector2>("Velocity");
			particleLife = particleData.Get<float>("Life");
			particleAge = particleData.Get<float>("Age");

			paramParticleLife = particleDefinition.GetFloatParameter("ParticleLife");
			paramSpawnRate = particleDefinition.GetFloatParameter("SpawnRate");
			paramOffsetX = particleDefinition.GetFloatParameter("OffsetX");
			paramOffsetY = particleDefinition.GetFloatParameter("OffsetY");
		}

		public override void Reload()
		{
			paramEmitterLife = particleDefinition.Parameters["EmitterLife"].Value.Get<float>();
		}

		public override void Clear()
		{
			particleSpawnAccumulator = 0;
		}

		public override void Update(Time time)
		{
			particleSpawnAccumulator += time.ElapsedSeconds;
			emitterAge += time.ElapsedSeconds;

			if (paramEmitterLife > 0 && emitterAge > paramEmitterLife)
			{
				emitterAge %= paramEmitterLife;
			}

			if (Alive)
			{
				int insaneCounter = 100;
				while (insaneCounter > 0)
				{
					insaneCounter--;

					var spawnRate = paramSpawnRate.Get(EmitterLifeFractional, 0);

					if (spawnRate > 0)
					{
						var secondsPerParticle = 1.0f / spawnRate;

						if (particleSpawnAccumulator >= secondsPerParticle)
						{
							Emit();
							particleSpawnAccumulator -= secondsPerParticle;
						}
						else
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
			}
		}

		protected int EmitInternal(Vector2 position, Vector2 velocity, float life)
		{
			if (particleData.ActiveParticles + 1 >= particleData.MaxParticles) particleData.Resize();
			var index = particleData.ActiveParticles;
			particleData.ActiveParticles++;

			var e = EmitterLifeFractional;

			particlePosition[index] = position + new Vector2(paramOffsetX.Get(e, 0), paramOffsetY.Get(e, 0));
			particleVelocity[index] = velocity;
			particleAge[index] = 0;
			particleLife[index] = paramParticleLife.Get(e, 0);
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
			paramDirection = particleDefinition.GetFloatParameter("Direction");
			paramRange = particleDefinition.GetFloatParameter("Range");
		}
		
		public override int Emit()
		{
			var e = EmitterLifeFractional;

			float range = paramRange.Get(e, 0) / 2;
			float direction = paramDirection.Get(e, 0) + MathTools.Random().NextFloat(-range, range);

			return EmitInternal(Point, MathTools.FromAngle(MathTools.ToRadians(direction)) * 40, 0);
		}
	}
}
