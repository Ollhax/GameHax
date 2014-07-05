using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Utility;
using System;

namespace MG.Framework.Particle
{
	public class ParticleSystem
	{
		public Vector2 Position;
		public Vector2 Gravity;
		
		public int ActiveParticles { get { return particleData.ActiveParticles; } }
		public readonly ParticleDefinition Definition;
		public List<ParticleSystem> SubSystems = new List<ParticleSystem>();
		
		private AssetHandler assetHandler;
		private ParticleSystemPool particleSystemPool;
		private Texture2D particleTexture;
		private List<Vector2> particlePosition;
		private List<Vector2> particleVelocity;
		private List<float> particleRotation;
		private List<float> particleRotationSpeed;
		private List<float> particleLife;
		private List<float> particleAge;
		private ParticleData particleData = new ParticleData(64);
		private ParticleEmitter emitter;
		private bool disabled;
		
		private ParticleDefinition.Parameter paramTexture;
		private Vector2 paramTextureAnchor;
		private Vector2I paramTextureCells;
		private float paramTextureFrameTime;
		private BlendMode paramBlendMode;
		private bool paramParticleInfinite;
		private bool paramParticleOrientToVelocity;
		private bool paramParticleRelativeToParent;
		private Gradient paramParticleColor;
		private RandomFloat paramParticleGravityScale;
		private RandomFloat paramParticleAccelerationX;
		private RandomFloat paramParticleAccelerationY;
		private RandomFloat paramParticleAccelerationAngular;
		private RandomFloat paramParticleAirResistance;
		private RandomFloat paramParticleTurn;
		private RandomFloat paramParticleScale;
		private RandomFloat paramParticleScaleX;
		private RandomFloat paramParticleScaleY;

		private int AnimationCells { get { return paramTextureCells.X * paramTextureCells.Y; } }

		public ParticleSystem(AssetHandler assetHandler, ParticleSystemPool particleSystemPool, ParticleDefinition particleDefinition)
		{
			if (assetHandler == null) throw new ArgumentException("assetHandler");
			if (particleSystemPool == null) throw new ArgumentException("particleSystemPool");
			if (particleDefinition == null) throw new ArgumentException("particleDefinition");

			this.assetHandler = assetHandler;
			this.particleSystemPool = particleSystemPool;
			this.Definition = particleDefinition;
			
			particlePosition = particleData.Register<Vector2>("Position");
			particleVelocity = particleData.Register<Vector2>("Velocity");
			particleRotation = particleData.Register<float>("Rotation");
			particleRotationSpeed = particleData.Register<float>("RotationSpeed");
			particleLife = particleData.Register<float>("Life");
			particleAge = particleData.Register<float>("Age");
			
			emitter = new PointEmitter(particleData, particleDefinition);
		}
				
		public void Reload()
		{
			bool wasRelative = paramParticleRelativeToParent;

			paramTexture = Definition.GetParameter("Texture");
			paramTextureAnchor = new Vector2(Definition.GetParameter("TextureAnchorX").Value.Get<float>(), Definition.GetParameter("TextureAnchorY").Value.Get<float>());
			paramTextureCells = new Vector2I(Definition.GetParameter("TextureCellsX").Value.Get<int>(), Definition.GetParameter("TextureCellsY").Value.Get<int>());
			paramTextureFrameTime = Definition.GetParameter("TextureFrameTime").Value.Get<float>();
			paramBlendMode = (BlendMode)Definition.GetParameter("BlendMode").Value.Get<int>();
			paramParticleInfinite = Definition.GetParameter("ParticleInfinite").Value.Get<bool>();
			paramParticleOrientToVelocity = Definition.GetParameter("ParticleOrientToVelocity").Value.Get<bool>();
			paramParticleRelativeToParent = Definition.GetParameter("ParticleRelativeToParent").Value.Get<bool>();
			paramParticleColor = Definition.GetParameter("ParticleColor").Value.Get<Gradient>();
			paramParticleGravityScale = Definition.GetFloatParameter("ParticleGravityScale");
			paramParticleAccelerationX = Definition.GetFloatParameter("ParticleAccelerationX");
			paramParticleAccelerationY = Definition.GetFloatParameter("ParticleAccelerationY");
			paramParticleAccelerationAngular = Definition.GetFloatParameter("ParticleAccelerationAngular");
			paramParticleAirResistance = Definition.GetFloatParameter("ParticleAirResistance");
			paramParticleTurn = Definition.GetFloatParameter("ParticleTurn");
			paramParticleScale = Definition.GetFloatParameter("ParticleScale");
			paramParticleScaleX = Definition.GetFloatParameter("ParticleScaleX");
			paramParticleScaleY = Definition.GetFloatParameter("ParticleScaleY");

			if (wasRelative != paramParticleRelativeToParent)
			{
				particleData.ActiveParticles = 0;
			}

			var texture = paramTexture.Value.Get<FilePath>();
			particleTexture = assetHandler.Load<Texture2D>(texture);
			emitter.Reload();

			if (Definition.Children.Count != SubSystems.Count)
			{
				ClearChildren();
				SubSystems.Capacity = Definition.Children.Count;
				foreach (var child in Definition.Children)
				{
					SubSystems.Add(particleSystemPool.Create(child));
				}
			}
			else
			{
				foreach (var child in SubSystems)
				{
					child.Reload();
				}
			}
		}

		public void SetPositionRecursive(Vector2 position)
		{
			Position = position;
			foreach (var child in SubSystems)
			{
				child.SetPositionRecursive(position);
			}
		}

		public void SetGravityRecursive(Vector2 gravity)
		{
			Gravity = gravity;
			foreach (var child in SubSystems)
			{
				child.SetGravityRecursive(gravity);
			}
		}

		public void Clear()
		{
			Position = Vector2.Zero;
			particleData.ActiveParticles = 0;
			emitter.Clear();
			disabled = false;

			ClearChildren();
		}

		private void ClearChildren()
		{
			foreach (var child in SubSystems)
			{
				child.Clear();
				particleSystemPool.Destroy(child);
			}
			SubSystems.Clear();
		}

		public bool Disabled
		{
			get { return disabled; }
			set
			{
				disabled = value;
				foreach (var system in SubSystems)
				{
					system.Disabled = true;
				}
			}
		}

		public bool Dead
		{
			get
			{
				if (!Disabled)
				{
					if (emitter.Alive) return false;
					if (paramParticleInfinite) return false;
				}
				
				if (particleData.ActiveParticles > 0) return false;

				foreach (var system in SubSystems)
				{
					if (!system.Dead) return false;
				}

				return true;
			}
		}

		public void Update(Time time)
		{
			if (time.ElapsedSeconds <= 0)
				return;

			if (!paramParticleRelativeToParent)
			{
				((PointEmitter)emitter).Point = Position;
			}
			else
			{
				((PointEmitter)emitter).Point = Vector2.Zero;
			}
			
			if (!Disabled)
			{
				emitter.Update(time);
			}
			
			for (int i = 0; i < particleData.ActiveParticles;)
			{
				var emitterLife = emitter.LifeFractional;
				var lifeFraction = particleAge[i] / particleLife[i];
				var accel = new Vector2(
					paramParticleAccelerationX.Get(emitterLife, lifeFraction),
					paramParticleAccelerationY.Get(emitterLife, lifeFraction));

				accel += Gravity * paramParticleGravityScale.Get(emitterLife, lifeFraction);

				var oldVel = particleVelocity[i];
				var vel = oldVel + accel * time.ElapsedSeconds;
				
				float resistance = paramParticleAirResistance.Get(emitterLife, lifeFraction);
				if (resistance != 0)
				{
					var res = vel * resistance * time.ElapsedSeconds;
					var absX = Math.Abs(vel.X);
					var absY = Math.Abs(vel.Y);
					res.X = MathTools.Clamp(res.X, -absX, absX);
					res.Y = MathTools.Clamp(res.Y, -absY, absY);
					vel -= res;
				}

				var turn = paramParticleTurn.Get(emitterLife, lifeFraction);
				if (turn != 0)
				{
					vel = vel.Rotated(MathTools.ToRadians(turn));
				}

				particleVelocity[i] = vel;
				particlePosition[i] += (oldVel + particleVelocity[i]) / 2 * time.ElapsedSeconds;
				
				if (paramParticleOrientToVelocity)
				{
					particleRotation[i] = vel.Angle();
				}
				else
				{
					particleRotationSpeed[i] += MathTools.ToRadians(paramParticleAccelerationAngular.Get(emitterLife, lifeFraction)) * time.ElapsedSeconds;
					particleRotation[i] += particleRotationSpeed[i] * time.ElapsedSeconds;
				}
				
				particleAge[i] += time.ElapsedSeconds;
				
				if (!paramParticleInfinite && particleAge[i] >= particleLife[i])
				{
					emitter.Destroy(i);
				}
				else
				{
					i++;
				}
			}

			foreach (var system in SubSystems)
			{
				system.Update(time);
			}
		}

		public void Draw(RenderContext renderContext)
		{
			Draw(renderContext, Matrix.Identity);
		}

		public void Draw(RenderContext renderContext, Matrix transform)
		{
			DrawCurrent(renderContext, transform);

			foreach (var system in SubSystems)
			{
				system.Draw(renderContext, transform);
			}
		}

		public void DrawCurrent(RenderContext renderContext, Matrix transform)
		{
			var quadBatch = renderContext.QuadBatch;

			// TODO: Figure out the best blending mode
			if (paramBlendMode == BlendMode.BlendmodeAlpha)
			{
				paramBlendMode = BlendMode.BlendmodeNonPremultiplied;
			}

			quadBatch.Begin(transform, paramBlendMode);

			for (int i = 0; i < particleData.ActiveParticles; i++)
			{
				var p = particlePosition[i];
				var a = particleAge[i];
				var l = particleLife[i];
				var r = particleRotation[i];
				var lifeFraction = a / l;
				var color = paramParticleColor.Evaluate(lifeFraction);
				var s = paramParticleScale.Get(emitter.LifeFractional, lifeFraction);
				var sx = paramParticleScaleX.Get(emitter.LifeFractional, lifeFraction);
				var sy = paramParticleScaleY.Get(emitter.LifeFractional, lifeFraction);
				
				if (paramParticleRelativeToParent)
				{
					p += Position;
				}

				var sourceArea = new RectangleF(0, 0, particleTexture.Width, particleTexture.Height);
				var maxCells = AnimationCells;
				var frameTime = paramTextureFrameTime;

				if (maxCells > 1 && paramTextureFrameTime != 0)
				{
					int frame = (int)(a / paramTextureFrameTime) % maxCells;
					int frameX = frame % paramTextureCells.X;
					int frameY = frame / paramTextureCells.X;
					float cellX = particleTexture.Width / (float)paramTextureCells.X;
					float cellY = particleTexture.Height / (float)paramTextureCells.Y;

					sourceArea = new RectangleF(frameX * cellX, frameY * cellY, cellX, cellY);
				}

				quadBatch.Draw(particleTexture, MathTools.Create2DAffineMatrix(p.X, p.Y, s * sx, s * sy, r), sourceArea, color, sourceArea.Size * paramTextureAnchor, QuadEffects.None, 0);
			}

			quadBatch.End();
		}
	}
}
