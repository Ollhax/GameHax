using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using MG.Framework.Graphics;
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

		public abstract float LifeFractional { get; }
		public abstract bool Alive { get; }
		public abstract void Update(Time time);
		public abstract int Emit();
		public abstract void Destroy(int index);
		public abstract void Clear();
		public abstract void Reload();
	}

	public abstract class BasicParticleEmitter : ParticleEmitter
	{
		enum EmitterLoopMode
		{
			Loop = 0,
			Infinite = 1,
			Once = 2
		}

		private EmitterLoopMode paramEmitterLoopMode;
		private float paramEmitterLife;
		private BlendMode paramBlendMode;
		private ParticleSortMode paramSortMode;
		private RandomFloat paramParticleLife;
		private RandomInt paramEmitterCount;
		private RandomFloat paramEmitterSpawnDelay;
		private RandomFloat paramEmitterSpawnRate;
		private RandomFloat paramEmitterOffsetX;
		private RandomFloat paramEmitterOffsetY;
		private RandomFloat paramEmitterInitialRotation;
		private RandomFloat paramEmitterInitialRotationSpeed;
		private List<Vector2> particlePosition;
		private List<Vector2> particleVelocity;
		private List<float> particleRotation;
		private List<float> particleRotationSpeed;
		private List<float> particleLife;
		private List<float> particleAge;

		private float emitterSpawnDelay;
		private float emitterAge;
		private float particleSpawnAccumulator;
		private int emitterCount;
		private int emitterCountMax;
		
		public override float LifeFractional { get { return paramEmitterLife > 0 ? emitterAge / paramEmitterLife : 0; } }
		public override bool Alive
		{
			get
			{
				return 
					paramEmitterLife > 0 &&
					(paramEmitterCount.BaseValue == 0 || emitterCount < emitterCountMax) &&
					(emitterAge < paramEmitterLife || paramEmitterLoopMode == EmitterLoopMode.Infinite);
			}
		}
		
		private ParticleSortMode SortMode
		{
			get { return paramBlendMode == BlendMode.BlendmodeAdditive ? ParticleSortMode.Unsorted : paramSortMode; }
		}

		public BasicParticleEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{
			particlePosition = particleData.Get<Vector2>("Position");
			particleVelocity = particleData.Get<Vector2>("Velocity");
			particleRotation = particleData.Get<float>("Rotation");
			particleRotationSpeed = particleData.Get<float>("RotationSpeed");
			particleLife = particleData.Get<float>("Life");
			particleAge = particleData.Get<float>("Age");
			
			paramParticleLife = particleDefinition.GetFloatParameter("ParticleLife");
			paramEmitterCount = particleDefinition.GetIntParameter("EmitterCount");
			paramEmitterSpawnDelay = particleDefinition.GetFloatParameter("EmitterSpawnDelay");
			paramEmitterSpawnRate = particleDefinition.GetFloatParameter("EmitterSpawnRate");
			paramEmitterOffsetX = particleDefinition.GetFloatParameter("EmitterOffsetX");
			paramEmitterOffsetY = particleDefinition.GetFloatParameter("EmitterOffsetY");
			paramEmitterInitialRotation = particleDefinition.GetFloatParameter("EmitterInitialRotation");
			paramEmitterInitialRotationSpeed = particleDefinition.GetFloatParameter("EmitterInitialRotationSpeed");
		}

		public override void Reload()
		{
			paramEmitterLife = particleDefinition.Parameters["EmitterLife"].Value.Get<float>();
			paramEmitterLoopMode = (EmitterLoopMode)particleDefinition.Parameters["EmitterLoop"].Value.Get<int>();
			paramBlendMode = (BlendMode)particleDefinition.Parameters["BlendMode"].Value.Get<int>();
			paramSortMode = (ParticleSortMode)particleDefinition.Parameters["SortMode"].Value.Get<int>();
			emitterCountMax = paramEmitterCount.Get(0, 0);
			emitterSpawnDelay = paramEmitterSpawnDelay.Get(0, 0);
		}

		public override void Clear()
		{
			emitterAge = 0;
			particleSpawnAccumulator = 0;
			emitterCount = 0;
		}

		public override void Update(Time time)
		{
			if (paramEmitterLife <= 0) return;

			if (emitterSpawnDelay > 0)
			{
				emitterSpawnDelay -= time.ElapsedSeconds;
				if (emitterSpawnDelay > 0) return;
			}

			particleSpawnAccumulator += time.ElapsedSeconds;
			emitterAge += time.ElapsedSeconds;

			if (emitterAge > paramEmitterLife && paramEmitterLoopMode == EmitterLoopMode.Loop)
			{
				emitterAge %= paramEmitterLife;
			}

			if (Alive)
			{
				int insaneCounter = 100;
				while (insaneCounter > 0)
				{
					insaneCounter--;

					var spawnRate = paramEmitterSpawnRate.Get(LifeFractional, 0);

					float secondsPerParticle = 0;
					if (spawnRate > 0)
					{
						secondsPerParticle = 1.0f / spawnRate;
					}

					if (particleSpawnAccumulator >= secondsPerParticle && Alive)
					{
						Emit();
						particleSpawnAccumulator -= secondsPerParticle;
						emitterCount++;
					}
					else
					{
						break;
					}
				}
			}
		}

		public override void Destroy(int index)
		{
			var sortMode = SortMode;

			if (sortMode != ParticleSortMode.Unsorted)
			{
				particleData.Shuffle(particleData.ActiveParticles - 1, index);
				particleData.ActiveParticles--;
				return;
			}

			particleData.Move(particleData.ActiveParticles - 1, index);
			particleData.ActiveParticles--;
		}
		
		protected int EmitInternal(Vector2 position, Vector2 velocity, float life)
		{
			if (particleData.ActiveParticles + 1 >= particleData.MaxParticles) particleData.Resize();
			var index = particleData.ActiveParticles;
			particleData.ActiveParticles++;

			var e = LifeFractional;
			float newParticleLife = paramParticleLife.Get(e, 0);
			var sortMode = SortMode;

			if (sortMode == ParticleSortMode.OldestOnTop)
			{
				index = 0;
				for (int i = 0; i < particleData.ActiveParticles - 1; i++)
				{
					if (particleAge[i] < newParticleLife)
					{
						index = i;
						break;
					}
				}
				particleData.Shuffle(index, particleData.ActiveParticles - 1);
			}
			else if (sortMode == ParticleSortMode.NewestOnTop)
			{
				index = particleData.ActiveParticles - 1;
				for (int i = particleData.ActiveParticles - 1; i >= 0; i--)
				{
					if (particleAge[i] < newParticleLife)
					{
						index = i;
						break;
					}
				}
				particleData.Shuffle(index, particleData.ActiveParticles - 1);
			}
			
			particlePosition[index] = position + new Vector2(paramEmitterOffsetX.Get(e, 0), paramEmitterOffsetY.Get(e, 0));
			particleVelocity[index] = velocity;
			particleRotation[index] = MathTools.ToRadians(paramEmitterInitialRotation.Get(e, 0));
			particleRotationSpeed[index] = MathTools.ToRadians(paramEmitterInitialRotationSpeed.Get(e, 0));
			particleAge[index] = 0;
			particleLife[index] = newParticleLife;
			
			//if (sortMode != ParticleSortMode.Unsorted)
			//{
			//    float last = particleAge[0];
			//    string s = "";
			//    for (int i = 0; i < particleData.ActiveParticles; i++)
			//    {
			//        s += particleAge[i].ToString("0.00", CultureInfo.InvariantCulture) + "  ";

			//        if ((sortMode == ParticleSortMode.NewestOnTop && particleAge[i] > last) ||
			//            (sortMode == ParticleSortMode.OldestOnTop && particleAge[i] < last))
			//        {
			//            Debug.WriteLine("out of order!");
			//        }
			//    }
			//    Debug.WriteLine(s);
			//}
			
			return index;
		}
	}

	public class PointEmitter : BasicParticleEmitter
	{
		public Vector2 Point;
		
		private RandomFloat paramDirection;
		private RandomFloat paramRange;
		private RandomFloat paramEmitterInitialSpeed;

		public PointEmitter(ParticleData particleData, ParticleDefinition particleDefinition)
			: base(particleData, particleDefinition)
		{
			paramDirection = particleDefinition.GetFloatParameter("EmitterDirection");
			paramRange = particleDefinition.GetFloatParameter("EmitterRange");
			paramEmitterInitialSpeed = particleDefinition.GetFloatParameter("EmitterInitialSpeed");
		}
		
		public override int Emit()
		{
			var e = LifeFractional;

			float range = paramRange.Get(e, 0) / 2;
			float direction = paramDirection.Get(e, 0) + MathTools.Random().NextFloat(-range, range);

			return EmitInternal(Point, MathTools.FromAngle(ParticleHelpers.ToRadians(direction)) * paramEmitterInitialSpeed.Get(e, 0), 0);
		}
	}
}
