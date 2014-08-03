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
			get { return group.Position; }
			set { group.Position = value; }
		}

		public Vector2 Gravity
		{
			get { return group.Gravity; }
			set { group.Gravity = value; }
		}

		public float Rotation
		{
			get { return group.Rotation; }
			set { group.Rotation = value; }
		}

		public bool EmitterDisabled
		{
			get { return group.EmitterDisabled; }
			set { group.EmitterDisabled = value; }
		}

		public double TimeSinceStart;
		public float EmitterSpawnDelay;
		public float EmitterAge;
		public float EmitterSpawnAccumulator;
		public int EmitterCount;
		public int EmitterCountMax;
		
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
		
		public readonly ParticleDefinition Definition;
		public List<ParticleEffect> SubSystems = new List<ParticleEffect>();
		public ParticleData ParticleData = new ParticleData(64);
		
		public readonly List<Vector2> ParticlePosition;
		public readonly List<Vector2> ParticleVelocity;
		public readonly List<float> ParticleRotation;
		public readonly List<float> ParticleRotationSpeed;
		public readonly List<float> ParticleScale;
		public readonly List<float> ParticleLife;
		public readonly List<float> ParticleAge;

		public Texture2D ParticleTexture;
		public Vector2 ParamTextureAnchor;
		public Vector2I ParamTextureCells;
		public float ParamTextureFrameTime;
		public BlendMode ParamBlendMode;
		public SortMode ParamSortMode;
		public bool ParamParticleInfinite;
		public bool ParamParticleOrientToVelocity;
		public bool ParamParticleRelativeToParent;
		public Gradient ParamParticleColor;
		public LoopMode ParamEmitterLoopMode;
		public float ParamEmitterLife;

		public RandomFloat ParamParticleGravityScale;
		public RandomFloat ParamParticleAccelerationX;
		public RandomFloat ParamParticleAccelerationY;
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
				if (!group.EmitterDisabled)
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
		private ParticleGroup group;

		public ParticleEffect(AssetHandler assetHandler, ParticleEffectPool particleEffectPool, ParticleDefinition particleDefinition)
		{
			if (assetHandler == null) throw new ArgumentException("assetHandler");
			if (particleEffectPool == null) throw new ArgumentException("particleEffectPool");
			if (particleDefinition == null) throw new ArgumentException("particleDefinition");

			this.assetHandler = assetHandler;
			this.particleEffectPool = particleEffectPool;
			this.Definition = particleDefinition;

			if (IsGroup) return;

			ParticlePosition = ParticleData.Register<Vector2>("Position");
			ParticleVelocity = ParticleData.Register<Vector2>("Velocity");
			ParticleRotation = ParticleData.Register<float>("Rotation");
			ParticleRotationSpeed = ParticleData.Register<float>("RotationSpeed");
			ParticleScale = ParticleData.Register<float>("Scale");
			ParticleLife = ParticleData.Register<float>("Life");
			ParticleAge = ParticleData.Register<float>("Age");
			
			ParamParticleGravityScale = Definition.GetFloatParameter("ParticleGravityScale");
			ParamParticleAccelerationX = Definition.GetFloatParameter("ParticleAccelerationX");
			ParamParticleAccelerationY = Definition.GetFloatParameter("ParticleAccelerationY");
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
			Reload(group);
		}

		public void Reload(ParticleGroup group)
		{
			this.group = group;
			bool wasRelative = ParamParticleRelativeToParent;
			
			if (!IsGroup)
			{
				ParamTextureAnchor = new Vector2(Definition.GetParameter("TextureAnchorX").Value.Get<float>(), Definition.GetParameter("TextureAnchorY").Value.Get<float>());
				ParamTextureCells = new Vector2I(Definition.GetParameter("TextureCellsX").Value.Get<int>(), Definition.GetParameter("TextureCellsY").Value.Get<int>());
				ParamTextureFrameTime = Definition.GetParameter("TextureFrameTime").Value.Get<float>();
				ParamBlendMode = (BlendMode)Definition.GetParameter("BlendMode").Value.Get<int>();
				ParamSortMode = (SortMode)Definition.Parameters["SortMode"].Value.Get<int>();
				ParamParticleInfinite = Definition.GetParameter("ParticleInfinite").Value.Get<bool>();
				ParamParticleOrientToVelocity = Definition.GetParameter("ParticleOrientToVelocity").Value.Get<bool>();
				ParamParticleRelativeToParent = Definition.GetParameter("ParticleRelativeToParent").Value.Get<bool>();
				ParamParticleColor = Definition.GetParameter("ParticleColor").Value.Get<Gradient>();
				ParamEmitterLife = Definition.Parameters["EmitterLife"].Value.Get<float>();
				ParamEmitterLoopMode = (LoopMode)Definition.Parameters["EmitterLoop"].Value.Get<int>();

				EmitterCountMax = ParamEmitterCount.Get(0, 0);
				EmitterSpawnDelay = ParamEmitterSpawnDelay.Get(0, 0);

				if (wasRelative != ParamParticleRelativeToParent)
				{
					ParticleData.ActiveParticles = 0;
				}

				var texture = Definition.GetParameter("Texture").Value.Get<FilePath>();
				ParticleTexture = assetHandler.Load<Texture2D>(texture);
			}
			
			if (Definition.Children.Count != SubSystems.Count)
			{
				ClearChildren();
				SubSystems.Capacity = Definition.Children.Count;
				foreach (var child in Definition.Children)
				{
					SubSystems.Add(particleEffectPool.Create(child, group));
				}
			}
			else
			{
				foreach (var child in SubSystems)
				{
					child.Reload(group);
				}
			}
		}
		
		public void Clear()
		{
			ParticleData.ActiveParticles = 0;
			TimeSinceStart = 0;
			EmitterSpawnDelay = 0;
			EmitterAge = 0;
			EmitterSpawnAccumulator = 0;
			EmitterCount = 0;
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
