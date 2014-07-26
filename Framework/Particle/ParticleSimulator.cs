using System;
using MG.Framework.Numerics;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public static class ParticleSimulator
	{
		public static void Update(ParticleEffect particleEffect, Time time)
		{
			UpdateInternal(particleEffect, time);
		}

		private static void UpdateInternal(ParticleEffect particleEffect, Time time)
		{
			particleEffect.TimeSinceStart = time.TotalElapsedSeconds;

			if (time.ElapsedSeconds <= 0)
				return;

			if (!particleEffect.Group.EmitterDisabled)
			{
				UpdateEmitter(particleEffect, time);
			}

			for (int i = 0; i < particleEffect.ParticleData.ActiveParticles; )
			{
				var emitterLife = particleEffect.LifeFractional;
				var lifeFraction = particleEffect.ParticleAge[i] / particleEffect.ParticleLife[i];
				var accel = new Vector2(
					particleEffect.ParamParticleAccelerationX.Get(emitterLife, lifeFraction),
					particleEffect.ParamParticleAccelerationY.Get(emitterLife, lifeFraction));

				accel += particleEffect.Group.Gravity * particleEffect.ParamParticleGravityScale.Get(emitterLife, lifeFraction);

				var oldVel = particleEffect.ParticleVelocity[i];
				var vel = oldVel + accel * time.ElapsedSeconds;

				float resistance = particleEffect.ParamParticleAirResistance.Get(emitterLife, lifeFraction);
				if (resistance != 0)
				{
					var res = vel * resistance * time.ElapsedSeconds;
					var absX = Math.Abs(vel.X);
					var absY = Math.Abs(vel.Y);
					res.X = MathTools.Clamp(res.X, -absX, absX);
					res.Y = MathTools.Clamp(res.Y, -absY, absY);
					vel -= res;
				}

				var turn = particleEffect.ParamParticleTurn.Get(emitterLife, lifeFraction);
				if (turn != 0)
				{
					vel = vel.Rotated(MathTools.ToRadians(turn));
				}

				var turbulence = particleEffect.ParamParticleTurbulenceStrength.Get(emitterLife, lifeFraction);
				if (turbulence != 0)
				{
					var v = SampleTurbulence(
						particleEffect,
						particleEffect.ParticlePosition[i],
						particleEffect.ParamParticleTurbulenceScale.Get(emitterLife, lifeFraction),
						particleEffect.ParamParticleTurbulenceSpeed.Get(emitterLife, lifeFraction));

					vel += v * turbulence;
				}

				particleEffect.ParticleVelocity[i] = vel;
				particleEffect.ParticlePosition[i] += (oldVel + particleEffect.ParticleVelocity[i]) / 2 * time.ElapsedSeconds;

				if (particleEffect.ParamParticleOrientToVelocity)
				{
					particleEffect.ParticleRotation[i] = vel.Angle() + MathTools.PiOver2;
				}
				else
				{
					particleEffect.ParticleRotationSpeed[i] += MathTools.ToRadians(particleEffect.ParamParticleAccelerationAngular.Get(emitterLife, lifeFraction)) * time.ElapsedSeconds;
					particleEffect.ParticleRotation[i] += particleEffect.ParticleRotationSpeed[i] * time.ElapsedSeconds;
				}

				particleEffect.ParticleAge[i] += time.ElapsedSeconds;

				if (!particleEffect.ParamParticleInfinite && particleEffect.ParticleAge[i] >= particleEffect.ParticleLife[i])
				{
					Destroy(particleEffect, i);
				}
				else
				{
					i++;
				}
			}

			foreach (var system in particleEffect.SubSystems)
			{
				Update(system, time);
			}
		}

		private static void UpdateEmitter(ParticleEffect particleEffect, Time time)
		{
			if (particleEffect.ParamEmitterLife <= 0) return;

			if (particleEffect.EmitterSpawnDelay > 0)
			{
				particleEffect.EmitterSpawnDelay -= time.ElapsedSeconds;
				if (particleEffect.EmitterSpawnDelay > 0) return;
			}

			particleEffect.EmitterSpawnAccumulator += time.ElapsedSeconds;
			particleEffect.EmitterAge += time.ElapsedSeconds;

			if (particleEffect.EmitterAge > particleEffect.ParamEmitterLife && particleEffect.ParamEmitterLoopMode == ParticleEffect.LoopMode.Loop)
			{
				particleEffect.EmitterAge %= particleEffect.ParamEmitterLife;
			}

			if (particleEffect.EmitterAlive)
			{
				int insaneCounter = 100;
				while (insaneCounter > 0)
				{
					insaneCounter--;

					var spawnRate = particleEffect.ParamEmitterSpawnRate.Get(particleEffect.LifeFractional, 0);

					float secondsPerParticle = 0;
					if (spawnRate > 0)
					{
						secondsPerParticle = 1.0f / spawnRate;
					}

					if (particleEffect.EmitterSpawnAccumulator >= secondsPerParticle && particleEffect.EmitterAlive)
					{
						var p = particleEffect.ParamParticleRelativeToParent ? Vector2.Zero : particleEffect.Group.Position;
						var e = particleEffect.LifeFractional;
						float range = particleEffect.ParamEmitterRange.Get(e, 0) / 2;
						float direction = MathTools.ToDegrees(particleEffect.Group.Rotation) + particleEffect.ParamEmitterDirection.Get(e, 0) + MathTools.Random().NextFloat(-range, range);
						
						EmitInternal(particleEffect, p, MathTools.FromAngle(ParticleHelpers.ToRadians(direction)) * particleEffect.ParamEmitterInitialSpeed.Get(e, 0), 0);
						
						particleEffect.EmitterSpawnAccumulator -= secondsPerParticle;
						particleEffect.EmitterCount++;
					}
					else
					{
						break;
					}
				}
			}
		}

		private static void Destroy(ParticleEffect particleEffect, int index)
		{
			var sortMode = particleEffect.EffectiveSortMode;

			if (sortMode != ParticleEffect.SortMode.Unsorted)
			{
				particleEffect.ParticleData.Shuffle(particleEffect.ParticleData.ActiveParticles - 1, index);
				particleEffect.ParticleData.ActiveParticles--;
				return;
			}

			particleEffect.ParticleData.Move(particleEffect.ParticleData.ActiveParticles - 1, index);
			particleEffect.ParticleData.ActiveParticles--;
		}
		
		private static int EmitInternal(ParticleEffect particleEffect, Vector2 position, Vector2 velocity, float life)
		{
			if (particleEffect.ParticleData.ActiveParticles + 1 >= particleEffect.ParticleData.MaxParticles) particleEffect.ParticleData.Resize();
			var index = particleEffect.ParticleData.ActiveParticles;
			particleEffect.ParticleData.ActiveParticles++;

			var e = particleEffect.LifeFractional;
			float newParticleLife = particleEffect.ParamParticleLife.Get(e, 0);
			var sortMode = particleEffect.EffectiveSortMode;

			if (sortMode == ParticleEffect.SortMode.OldestOnTop)
			{
				index = 0;
				for (int i = 0; i < particleEffect.ParticleData.ActiveParticles - 1; i++)
				{
					if (particleEffect.ParticleAge[i] < newParticleLife)
					{
						index = i;
						break;
					}
				}
				particleEffect.ParticleData.Shuffle(index, particleEffect.ParticleData.ActiveParticles - 1);
			}
			else if (sortMode == ParticleEffect.SortMode.NewestOnTop)
			{
				index = particleEffect.ParticleData.ActiveParticles - 1;
				for (int i = particleEffect.ParticleData.ActiveParticles - 1; i >= 0; i--)
				{
					if (particleEffect.ParticleAge[i] < newParticleLife)
					{
						index = i;
						break;
					}
				}
				particleEffect.ParticleData.Shuffle(index, particleEffect.ParticleData.ActiveParticles - 1);
			}

			particleEffect.ParticlePosition[index] = position + new Vector2(particleEffect.ParamEmitterOffsetX.Get(e, 0), particleEffect.ParamEmitterOffsetY.Get(e, 0));
			particleEffect.ParticleVelocity[index] = velocity;
			particleEffect.ParticleRotation[index] = MathTools.ToRadians(particleEffect.ParamEmitterInitialRotation.Get(e, 0));
			particleEffect.ParticleRotationSpeed[index] = MathTools.ToRadians(particleEffect.ParamEmitterInitialRotationSpeed.Get(e, 0));
			particleEffect.ParticleScale[index] = particleEffect.ParamEmitterInitialScale.Get(e, 0);
			particleEffect.ParticleAge[index] = 0;
			particleEffect.ParticleLife[index] = newParticleLife;

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
		
		private static Vector2 SampleTurbulence(ParticleEffect particleEffect, Vector2 position, float scale, float speed)
		{
			float d = scale;
			float s = 1.0f;
			float t = (float)(particleEffect.TimeSinceStart * speed);
			float x = position.X * d;
			float y = position.Y * d;

			float tl = NoiseTools.BasicNoise(x - s, y - s, t);
			float tr = NoiseTools.BasicNoise(x + s, y - s, t);
			float bl = NoiseTools.BasicNoise(x - s, y + s, t);
			float br = NoiseTools.BasicNoise(x + s, y + s, t);

			return new Vector2((tl + bl) - (tr + br), (tl + tr) - (bl + br));
		}
	}
}
