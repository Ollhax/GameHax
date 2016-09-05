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
			if (time.ElapsedSeconds <= 0 || particleEffect.ParamQualityLevel > ParticleDefinition.GlobalQualityLevel)
				return;

			particleEffect.TimeSinceStart += time.ElapsedSeconds;

			if (!particleEffect.EmitterDisabled)
			{
				UpdateEmitter(particleEffect, time);
			}

			for (int i = 0; i < particleEffect.ParticleData.ActiveParticles; i++)
			{
				int segmentIndex = particleEffect.ParticleSegmentIndex[i];
				if (segmentIndex >= 0 && (particleEffect.SegmentTransforms == null || segmentIndex >= particleEffect.SegmentTransforms.Count))
				{
					segmentIndex = -1;
				}

				var emitterLife = particleEffect.LifeFractional;
				var lifeFraction = particleEffect.ParticleAge[i] / particleEffect.ParticleLife[i];

				float resistance = particleEffect.ParamParticleAirResistance.Get(emitterLife, lifeFraction);
				var turn = particleEffect.ParamParticleTurn.Get(emitterLife, lifeFraction);

				// Gravity and world acceleration are absolute and need to be broken out
				var worldAccel = new Vector2(
					particleEffect.ParamParticleAccelerationX.Get(emitterLife, lifeFraction),
					particleEffect.ParamParticleAccelerationY.Get(emitterLife, lifeFraction));
				worldAccel += particleEffect.Gravity * particleEffect.ParamParticleGravityScale.Get(emitterLife, lifeFraction) * 100.0f;
				var oldWorldVelocity = particleEffect.ParticleWorldForceVelocity[i];
				var velWorld = oldWorldVelocity + worldAccel * time.ElapsedSeconds;
				if (resistance != 0)
				{
					var res = velWorld * resistance * time.ElapsedSeconds;
					var absX = Math.Abs(velWorld.X);
					var absY = Math.Abs(velWorld.Y);
					res.X = MathTools.Clamp(res.X, -absX, absX);
					res.Y = MathTools.Clamp(res.Y, -absY, absY);
					velWorld -= res;
				}
				if (turn != 0)
				{
					velWorld = velWorld.Rotated(MathTools.ToRadians(turn));
				}

				particleEffect.ParticleWorldForceVelocity[i] = velWorld;
				particleEffect.ParticleWorldForceOffset[i] += (oldWorldVelocity + velWorld) / 2 * time.ElapsedSeconds;

				var localAccel = new Vector2(
					particleEffect.ParamParticleAccelerationLocalX.Get(emitterLife, lifeFraction),
					particleEffect.ParamParticleAccelerationLocalY.Get(emitterLife, lifeFraction));
				var accel = new Vector2(0.0f, 0.0f);
				if (!localAccel.IsZero)
				{
					Vector2 localY = new Vector2(0.0f, -1.0f);
					localY = localY.Rotated(particleEffect.ParticleRotation[i]);
					Vector2 localX = new Vector2(1.0f, 0.0f);
					localX = localX.Rotated(particleEffect.ParticleRotation[i]);
					localAccel = localX * localAccel.X + localY * localAccel.Y;
					accel += localAccel;
				}

				var oldVel = particleEffect.ParticleVelocity[i];
				var vel = oldVel + accel * time.ElapsedSeconds;

				if (resistance != 0)
				{
					var res = vel * resistance * time.ElapsedSeconds;
					var absX = Math.Abs(vel.X);
					var absY = Math.Abs(vel.Y);
					res.X = MathTools.Clamp(res.X, -absX, absX);
					res.Y = MathTools.Clamp(res.Y, -absY, absY);
					vel -= res;
				}

				if (turn != 0)
				{
					vel = vel.Rotated(MathTools.ToRadians(turn));
				}

				var turbulence = particleEffect.ParamParticleTurbulenceStrength.Get(emitterLife, lifeFraction);
				if (turbulence != 0)
				{
					Vector2 particlePos = particleEffect.ParticlePosition[i];
					if (segmentIndex >= 0 && particleEffect.ParamSegmentSpawnType != ParticleEffect.SegmentSpawnType.CloneAll)
					{
						// Calculate final world position to get different turbulence for the different copies
						Matrix particleTransform = MathTools.Create2DAffineMatrix(particlePos.X, particlePos.Y, 1.0f, 1.0f, 0.0f);
						Matrix segmentTransform = particleEffect.SegmentTransforms[segmentIndex];
						particleTransform = particleTransform * segmentTransform;
						particlePos = particleTransform.TranslationXY;
					}
					if (particleEffect.ParamParticleRelativeToParent)
					{
						particlePos += particleEffect.Position;
					}
					else
					{
						particlePos += particleEffect.ParticleOrigin[i];
					}
					var v = SampleTurbulence(
						particleEffect,
						particlePos,
						particleEffect.ParamParticleTurbulenceScale.Get(emitterLife, lifeFraction) * 0.01f,
						particleEffect.ParamParticleTurbulenceSpeed.Get(emitterLife, lifeFraction) * 0.1f);

					vel += v * turbulence;
				}

				particleEffect.ParticleVelocity[i] = vel;
				particleEffect.ParticlePosition[i] += (oldVel + particleEffect.ParticleVelocity[i]) / 2 * time.ElapsedSeconds;

				if (particleEffect.ParamParticleOrientToVelocity)
				{
					if (particleEffect.ParticleWorldForceVelocity[i].IsZero)
					{
						particleEffect.ParticleRotation[i] = vel.Angle() + MathTools.PiOver2;
					}
					else if (segmentIndex >= 0)
					{
						Matrix segmentTransform = particleEffect.SegmentTransforms[segmentIndex];
						Matrix velocityMatrix = MathTools.Create2DAffineMatrix(particleEffect.ParticleWorldForceVelocity[i].X, particleEffect.ParticleWorldForceVelocity[i].Y, 1.0f, 1.0f, 0.0f);
						velocityMatrix = velocityMatrix * Matrix.Invert(segmentTransform);
						particleEffect.ParticleRotation[i] = (velocityMatrix.TranslationXY + particleEffect.ParticleVelocity[i]).Angle() + MathTools.PiOver2;
					}
					else
					{
						particleEffect.ParticleRotation[i] = (particleEffect.ParticleWorldForceVelocity[i] + particleEffect.ParticleVelocity[i]).Angle() + MathTools.PiOver2;
					}
				}
				else
				{
					particleEffect.ParticleRotationSpeed[i] += MathTools.ToRadians(particleEffect.ParamParticleAccelerationAngular.Get(emitterLife, lifeFraction)) * time.ElapsedSeconds;
					float rotationSpeed = particleEffect.ParticleRotationSpeed[i];
					particleEffect.ParticleRotation[i] += rotationSpeed * time.ElapsedSeconds;
				}

				particleEffect.ParticleAge[i] += time.ElapsedSeconds;

				if (particleEffect.ParticleAge[i] >= particleEffect.ParticleLife[i])
				{
					if (!particleEffect.ParamParticleInfinite)
					{
						Destroy(particleEffect, i);
						i--;
					}
					else
					{
						particleEffect.ParticleAge[i] -= particleEffect.ParticleLife[i];
					}
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
				particleEffect.RestartEmitter();
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
						var p = particleEffect.ParamParticleRelativeToParent ? Vector2.Zero : particleEffect.Position;
						var e = particleEffect.LifeFractional;
						float range = particleEffect.ParamEmitterRange.Get(e, 0) / 2;
						float rotationDirection = particleEffect.ParamParticleRelativeToParent ? 0.0f : MathTools.ToDegrees(particleEffect.Rotation);
						float mainDirection = rotationDirection + particleEffect.ParamEmitterDirection.Get(e, 0);
						float randomDirection = MathTools.Random().NextFloat(-range, range);
						float direction = mainDirection + randomDirection;
						int spawnSegment = 0;

						if (particleEffect.SegmentTransforms != null)
						{
							spawnSegment = 1;
							if (particleEffect.ParamSegmentSpawnType == ParticleEffect.SegmentSpawnType.RandomSegment)
							{
								spawnSegment += MathTools.Random().Next(particleEffect.SegmentTransforms.Count);
							}
							else if (particleEffect.ParamSegmentSpawnType == ParticleEffect.SegmentSpawnType.SequentialSegment)
							{
								if (particleEffect.LastSegment < 0)
								{
									particleEffect.LastSegment = 0;
								}
								spawnSegment = particleEffect.LastSegment + 1;
								if (particleEffect.LastSegment >= particleEffect.SegmentTransforms.Count)
								{
									spawnSegment = 1;
								}
								particleEffect.LastSegment = spawnSegment;
							}
						}
						EmitInternal(particleEffect, p, MathTools.FromAngle(ParticleHelpers.ToRadians(direction)) * particleEffect.ParamEmitterInitialSpeed.Get(e, 0), particleEffect.Rotation, 0, spawnSegment - 1);
						
						particleEffect.EmitterSpawnAccumulator -= secondsPerParticle;
						particleEffect.EmitterCount++;

						if (particleEffect.SegmentTransforms != null && particleEffect.SegmentTransforms.Count > 0 && particleEffect.ParamSegmentSpawnType == ParticleEffect.SegmentSpawnType.All)
						{
							for (int i = 1; i < particleEffect.SegmentTransforms.Count; ++i)
							{
								direction = mainDirection + MathTools.Random().NextFloat(-range, range);
								EmitInternal(particleEffect, p, MathTools.FromAngle(ParticleHelpers.ToRadians(direction)) * particleEffect.ParamEmitterInitialSpeed.Get(e, 0), particleEffect.Rotation, 0, i);
							}
						}
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
		
		private static int EmitInternal(ParticleEffect particleEffect, Vector2 position, Vector2 velocity, float rotation, float life, int segmentIndex)
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

			particleEffect.ParticleSegmentIndex[index] = segmentIndex;
			particleEffect.ParticleStartFrame[index] = MathTools.Clamp(Math.Abs(particleEffect.ParamTextureFrameStart.Get(e, 0)), 0.0f, 1.0f - MathTools.Epsilon); // 1.0 should be interpreted as last frame.
			Vector2 posOffset = new Vector2(particleEffect.ParamEmitterOffsetX.Get(e, 0), particleEffect.ParamEmitterOffsetY.Get(e, 0));
			float initialRotation = MathTools.ToRadians(particleEffect.ParamEmitterInitialRotation.Get(e, 0));
			if (!particleEffect.ParamParticleRelativeToParent && rotation != 0)
			{
				posOffset = posOffset.Rotated(rotation);
				initialRotation += rotation;
			}
			float initialScale = particleEffect.ParamEmitterInitialScale.Get(e, 0);
			particleEffect.ParticleOrigin[index] = position;
			particleEffect.ParticlePosition[index] = posOffset;
			particleEffect.ParticleVelocity[index] = velocity;
			particleEffect.ParticleWorldForceOffset[index] = Vector2.Zero;
			particleEffect.ParticleWorldForceVelocity[index] = Vector2.Zero;
			particleEffect.ParticleRotation[index] = initialRotation;
			particleEffect.ParticleRotationSpeed[index] = MathTools.ToRadians(particleEffect.ParamEmitterInitialRotationSpeed.Get(e, 0));
			particleEffect.ParticleScale[index] = initialScale;
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
