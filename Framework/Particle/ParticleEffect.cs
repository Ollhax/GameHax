using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Utility;
using System;

namespace MG.Framework.Particle
{
	public class ParticleEffect
	{
		public Vector2 Position
		{
			get { return common.Position; }
			set { common.Position = value; }
		}

		public Vector2 Gravity
		{
			get { return common.Gravity; }
			set { common.Gravity = value; }
		}

		public float Rotation
		{
			get { return common.Rotation; }
			set { common.Rotation = value; }
		}

		public bool EmitterDisabled
		{
			get { return common.EmitterDisabled; }
			set { common.EmitterDisabled = value; }
		}

		public double TimeSinceStart;
		public float EmitterSpawnDelay;
		public float EmitterAge;
		public float EmitterSpawnAccumulator;
		public int EmitterCount;
		public int EmitterCountMax;
		public int LastSegment;
		
		public enum LoopMode
		{
			Loop = 0,
			Infinite = 1,
			Once = 2
		}

		public enum SortMode
		{
			Unsorted,
			NewestOnTop,
			OldestOnTop,
		}

		public enum SegmentSpawnType
		{
			All,
			CloneAll,
			SequentialSegment,
			RandomSegment,
		}

		public readonly ParticleDefinition Definition;
		public List<ParticleEffect> SubSystems = new List<ParticleEffect>();
		public ParticleData ParticleData = new ParticleData(64);

		public readonly List<Vector2> ParticleOrigin;
		public readonly List<Vector2> ParticlePosition;
		public readonly List<Vector2> ParticleVelocity;
		public readonly List<Vector2> ParticleWorldForceOffset;
		public readonly List<Vector2> ParticleWorldForceVelocity;
		public readonly List<float> ParticleRotation;
		public readonly List<float> ParticleRotationSpeed;
		public readonly List<float> ParticleScale;
		public readonly List<float> ParticleLife;
		public readonly List<float> ParticleAge;
		public readonly List<float> ParticleStartFrame;
		public readonly List<int> ParticleSegmentIndex;

		public List<Matrix> SegmentTransforms;

		public Texture2D ParticleTexture;
		public int ParamQualityLevel;
		public Vector2 ParamTextureAnchor;
		public Vector2I ParamTextureCells;
		public BlendMode ParamBlendMode;
		public SortMode ParamSortMode;
		public bool ParamParticleInfinite;
		public bool ParamParticleOrientToVelocity;
		public bool ParamParticleRelativeToParent;
		public Gradient ParamParticleColor;
		public LoopMode ParamEmitterLoopMode;
		public float ParamEmitterLife;
		public int ParamSegmentCount;
		public float ParamSegmentCenter;
		public float ParamSegmentRange;
		public SegmentSpawnType ParamSegmentSpawnType;
		public bool ParamSegmentKeepRotation;
		public bool ParamMirror;
		public float ParamMirrorCenter;

		public RandomFloat ParamTextureFrameStart;
		public RandomFloat ParamTextureFrameTimeline;

		public RandomFloat ParamParticleGravityScale;
		public RandomFloat ParamParticleAccelerationX;
		public RandomFloat ParamParticleAccelerationY;
		public RandomFloat ParamParticleAccelerationLocalX;
		public RandomFloat ParamParticleAccelerationLocalY;
		public RandomFloat ParamParticleAccelerationAngular;
		public RandomFloat ParamParticleAirResistance;
		public RandomFloat ParamParticleTurn;
		public RandomFloat ParamParticleScale;
		public RandomFloat ParamParticleScaleX;
		public RandomFloat ParamParticleScaleY;
		public RandomFloat ParamParticleTurbulenceStrength;
		public RandomFloat ParamParticleTurbulenceScale;
		public RandomFloat ParamParticleTurbulenceSpeed;
		public RandomFloat ParamParticleLife;
		
		public RandomInt ParamEmitterCount;
		public RandomFloat ParamEmitterDirection;
		public RandomFloat ParamEmitterRange;
		public RandomFloat ParamEmitterInitialSpeed;
		public RandomFloat ParamEmitterSpawnDelay;
		public RandomFloat ParamEmitterSpawnRate;
		public RandomFloat ParamEmitterOffsetX;
		public RandomFloat ParamEmitterOffsetY;
		public RandomFloat ParamEmitterInitialRotation;
		public RandomFloat ParamEmitterInitialRotationSpeed;
		public RandomFloat ParamEmitterInitialScale;

		public float LifeFractional
		{
			get { return ParamEmitterLife > 0 ? EmitterAge / ParamEmitterLife : 0; }
		}

		public SortMode EffectiveSortMode
		{
			get { return ParamBlendMode == BlendMode.BlendmodeAdditive ? SortMode.Unsorted : ParamSortMode; }
		}

		public int AnimationCells
		{
			get { return ParamTextureCells.X * ParamTextureCells.Y; }
		}

		public bool Dead
		{
			get
			{
				if (!common.EmitterDisabled)
				{
					if (EmitterAlive) return false;
					if (ParamParticleInfinite) return false;
				}

				if (ParticleData.ActiveParticles > 0) return false;

				foreach (var system in SubSystems)
				{
					if (!system.Dead) return false;
				}

				return true;
			}
		}

		public bool EmitterAlive
		{
			get
			{
				return
					ParamEmitterLife > 0 &&
					(ParamEmitterCount.IsZero || EmitterCount < EmitterCountMax) &&
					(EmitterAge < ParamEmitterLife || ParamEmitterLoopMode == LoopMode.Infinite);
			}
		}

		public bool IsGroup
		{
			get { return Definition.Declaration == "Group"; }
		}
		
		private AssetHandler assetHandler;
		private ParticleEffectPool particleEffectPool;
		private ParticleCommon common;

		public ParticleEffect(AssetHandler assetHandler, ParticleEffectPool particleEffectPool, ParticleDefinition particleDefinition)
		{
			if (assetHandler == null) throw new ArgumentException("assetHandler");
			if (particleEffectPool == null) throw new ArgumentException("particleEffectPool");
			if (particleDefinition == null) throw new ArgumentException("particleDefinition");

			this.assetHandler = assetHandler;
			this.particleEffectPool = particleEffectPool;
			this.Definition = particleDefinition;

			if (IsGroup) return;

			ParticleOrigin = ParticleData.Register<Vector2>("Origin");
			ParticlePosition = ParticleData.Register<Vector2>("Position");
			ParticleVelocity = ParticleData.Register<Vector2>("Velocity");
			ParticleWorldForceOffset = ParticleData.Register<Vector2>("WorldForceOffset");
			ParticleWorldForceVelocity = ParticleData.Register<Vector2>("WorldForceVelocity");
			ParticleRotation = ParticleData.Register<float>("Rotation");
			ParticleRotationSpeed = ParticleData.Register<float>("RotationSpeed");
			ParticleScale = ParticleData.Register<float>("Scale");
			ParticleLife = ParticleData.Register<float>("Life");
			ParticleAge = ParticleData.Register<float>("Age");
			ParticleStartFrame = ParticleData.Register<float>("StartFrame");
			ParticleSegmentIndex = ParticleData.Register<int>("SegmentIndex");

			ParamTextureFrameStart = Definition.GetFloatParameter("TextureFrameStart");
			ParamTextureFrameTimeline = Definition.GetFloatParameter("TextureFrameTimeline");

			ParamParticleGravityScale = Definition.GetFloatParameter("ParticleGravityScale");
			ParamParticleAccelerationX = Definition.GetFloatParameter("ParticleAccelerationX");
			ParamParticleAccelerationY = Definition.GetFloatParameter("ParticleAccelerationY");
			ParamParticleAccelerationLocalX = Definition.GetFloatParameter("ParticleAccelerationLocalX");
			ParamParticleAccelerationLocalY = Definition.GetFloatParameter("ParticleAccelerationLocalY");
			ParamParticleAccelerationAngular = Definition.GetFloatParameter("ParticleAccelerationAngular");
			ParamParticleAirResistance = Definition.GetFloatParameter("ParticleAirResistance");
			ParamParticleTurn = Definition.GetFloatParameter("ParticleTurn");
			ParamParticleScale = Definition.GetFloatParameter("ParticleScale");
			ParamParticleScaleX = Definition.GetFloatParameter("ParticleScaleX");
			ParamParticleScaleY = Definition.GetFloatParameter("ParticleScaleY");
			ParamParticleTurbulenceStrength = Definition.GetFloatParameter("ParticleTurbulenceStrength");
			ParamParticleTurbulenceScale = Definition.GetFloatParameter("ParticleTurbulenceScale");
			ParamParticleTurbulenceSpeed = Definition.GetFloatParameter("ParticleTurbulenceSpeed");

			ParamEmitterCount = Definition.GetIntParameter("EmitterCount");
			ParamEmitterDirection = Definition.GetFloatParameter("EmitterDirection");
			ParamEmitterRange = Definition.GetFloatParameter("EmitterRange");
			ParamEmitterInitialSpeed = Definition.GetFloatParameter("EmitterInitialSpeed");
			ParamParticleLife = Definition.GetFloatParameter("ParticleLife");
			ParamEmitterSpawnDelay = Definition.GetFloatParameter("EmitterSpawnDelay");
			ParamEmitterSpawnRate = Definition.GetFloatParameter("EmitterSpawnRate");
			ParamEmitterOffsetX = Definition.GetFloatParameter("EmitterOffsetX");
			ParamEmitterOffsetY = Definition.GetFloatParameter("EmitterOffsetY");
			ParamEmitterInitialRotation = Definition.GetFloatParameter("EmitterInitialRotation");
			ParamEmitterInitialRotationSpeed = Definition.GetFloatParameter("EmitterInitialRotationSpeed");
			ParamEmitterInitialScale = Definition.GetFloatParameter("EmitterInitialScale");
		}

		public void Reload()
		{
			Reload(common);
		}

		private static Matrix CreateReflectionZ(float radians)
		{
			Matrix matrix;
			float num2 = (float)Math.Cos(2.0f * (double)radians);
			float num = (float)Math.Sin(2.0f * (double)radians);
			matrix.M11 = num2;
			matrix.M12 = num;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = num;
			matrix.M22 = -num2;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public void Reload(ParticleCommon common)
		{
			this.common = common;
			bool wasRelative = ParamParticleRelativeToParent;
			
			if (!IsGroup)
			{
				ParamQualityLevel = Definition.Parameters["QualityLevel"].Value.Get<int>();
				ParamTextureAnchor = new Vector2(Definition.GetParameter("TextureAnchorX").Value.Get<float>(), Definition.GetParameter("TextureAnchorY").Value.Get<float>());
				ParamTextureCells = new Vector2I(Definition.GetParameter("TextureCellsX").Value.Get<int>(), Definition.GetParameter("TextureCellsY").Value.Get<int>());
				ParamBlendMode = (BlendMode)Definition.GetParameter("BlendMode").Value.Get<int>();
				ParamSortMode = (SortMode)Definition.Parameters["SortMode"].Value.Get<int>();
				ParamParticleInfinite = Definition.GetParameter("ParticleInfinite").Value.Get<bool>();
				ParamParticleOrientToVelocity = Definition.GetParameter("ParticleOrientToVelocity").Value.Get<bool>();
				ParamParticleRelativeToParent = Definition.GetParameter("ParticleRelativeToParent").Value.Get<bool>();
				ParamParticleColor = Definition.GetParameter("ParticleColor").Value.Get<Gradient>();
				ParamEmitterLife = Definition.Parameters["EmitterLife"].Value.Get<float>();
				ParamEmitterLoopMode = (LoopMode)Definition.Parameters["EmitterLoop"].Value.Get<int>();

				RestartEmitter();

				if (wasRelative != ParamParticleRelativeToParent || ParamParticleInfinite)
				{
					Reset();
				}

				var texture = Definition.GetParameter("Texture").Value.Get<FilePath>();
				ParticleTexture = assetHandler.Load<Texture2D>(texture);
				LastSegment = -1;

				ParamSegmentCount = Definition.GetParameter("SegmentCount").Value.Get<int>();
				ParamSegmentCenter = Definition.GetParameter("SegmentCenter").Value.Get<float>();
				ParamSegmentRange = Definition.GetParameter("SegmentRange").Value.Get<float>();
				ParamSegmentSpawnType = (SegmentSpawnType)Definition.GetParameter("SegmentSpawnType").Value.Get<int>();
				ParamSegmentKeepRotation = Definition.GetParameter("SegmentKeepRotation").Value.Get<bool>();
				ParamMirror = Definition.GetParameter("Mirror").Value.Get<bool>();
				ParamMirrorCenter = Definition.GetParameter("MirrorCenter").Value.Get<float>();

				SegmentTransforms = null;
				if (ParamSegmentCount > 1)
				{
					SegmentTransforms = new List<Matrix>();
					float anglePerSegment = ParamSegmentRange * 2.0f / ParamSegmentCount;
					float segmentStart = -ParamSegmentRange + anglePerSegment / 2.0f;
					for (int i = 0; i < ParamSegmentCount; ++i)
					{
						SegmentTransforms.Add(Matrix.CreateRotationZ(MathTools.ToRadians(ParamSegmentCenter + segmentStart + i * anglePerSegment)));
					}
				}
				else if (ParamMirror)
				{
					SegmentTransforms = new List<Matrix>();
					SegmentTransforms.Add(Matrix.Identity);
				}
				if (ParamMirror)
				{
					Matrix mirrorMatrix = CreateReflectionZ(ParticleHelpers.ToRadians(ParamMirrorCenter));
					int segments = SegmentTransforms.Count;
					for (int i = 0; i < segments; ++i)
					{
						SegmentTransforms.Add(SegmentTransforms[i] * mirrorMatrix);
					}
				}
			}
			
			if (Definition.Children.Count != SubSystems.Count)
			{
				ClearChildren();
				SubSystems.Capacity = Definition.Children.Count;
				foreach (var child in Definition.Children)
				{
					SubSystems.Add(particleEffectPool.Create(child, common));
				}
			}
			else
			{
				foreach (var child in SubSystems)
				{
					child.Reload(common);
				}
			}
		}

		public void Reset()
		{
			ParticleData.ActiveParticles = 0;
			TimeSinceStart = 0;
			EmitterAge = 0;
			EmitterSpawnAccumulator = 0;
			EmitterCount = 0;
			LastSegment = -1;
		}

		public void RestartEmitter()
		{
			EmitterSpawnAccumulator = 0;
			EmitterCount = 0;
			EmitterCountMax = ParamEmitterCount.Get(0, 0);
			EmitterSpawnDelay = ParamEmitterSpawnDelay.Get(0, 0);
		}

		public void Clear()
		{
			Reset();
			
			EmitterSpawnDelay = 0;
			EmitterCountMax = 0;

			ClearChildren();
		}

		private void ClearChildren()
		{
			foreach (var child in SubSystems)
			{
				child.Clear();
				particleEffectPool.Destroy(child);
			}
			SubSystems.Clear();
		}
	}
}
