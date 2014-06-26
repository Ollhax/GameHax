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
		public float Angle;
		
		public int ActiveParticles { get { return particleData.ActiveParticles; } }
		public readonly ParticleDefinition Definition;
		public List<ParticleSystem> SubSystems = new List<ParticleSystem>();

		private AssetHandler assetHandler;
		private ParticleManager particleManager;
		private Texture2D particleTexture;
		private List<float> particleAge;
		
		private ParticleData particleData = new ParticleData(64);
		private ParticleEmitter emitter;
		
		private ParticleDefinition.Parameter paramTexture;
		private BlendMode paramBlendMode;
		private bool paramParticleInfinite;
		private Gradient paramParticleColor;
		private RandomFloat paramParticleScale;
		private RandomFloat paramParticleScaleX;
		private RandomFloat paramParticleScaleY;

		public ParticleSystem(AssetHandler assetHandler, ParticleManager particleManager, ParticleDefinition particleDefinition)
		{
			if (assetHandler == null) throw new ArgumentException("assetHandler");
			if (particleManager == null) throw new ArgumentException("particleManager");
			if (particleDefinition == null) throw new ArgumentException("particleDefinition");

			this.assetHandler = assetHandler;
			this.particleManager = particleManager;
			this.Definition = particleDefinition;
			
			particleData.Register<Vector2>("Position");
			particleData.Register<Vector2>("Velocity");
			particleData.Register<float>("Life");
			particleAge = particleData.Register<float>("Age");
			
			emitter = new PointEmitter(particleData, particleDefinition);
		}
		
		public void Reload()
		{
			paramTexture = Definition.Parameters["Texture"];
			paramBlendMode = (BlendMode)Definition.Parameters["BlendMode"].Value.Get<int>();
			paramParticleInfinite = Definition.Parameters["ParticleInfinite"].Value.Get<bool>();
			paramParticleColor = Definition.Parameters["ParticleColor"].Value.Get<Gradient>();
			paramParticleScale = Definition.GetFloatParameter("ParticleScale");
			paramParticleScaleX = Definition.GetFloatParameter("ParticleScaleX");
			paramParticleScaleY = Definition.GetFloatParameter("ParticleScaleY");

			var texture = paramTexture.Value.Get<FilePath>();
			particleTexture = assetHandler.Load<Texture2D>(texture);
			emitter.Reload();

			if (Definition.Children.Count != SubSystems.Count)
			{
				ClearChildren();
				SubSystems.Capacity = Definition.Children.Count;
				foreach (var child in Definition.Children)
				{
					SubSystems.Add(particleManager.Create(child));
				}
			}
		}

		public void Clear()
		{
			Position = Vector2.Zero;
			particleData.ActiveParticles = 0;
			emitter.Clear();

			ClearChildren();
		}

		private void ClearChildren()
		{
			foreach (var child in SubSystems)
			{
				child.Clear();
				particleManager.Destroy(child);
			}
			SubSystems.Clear();
		}
		
		public bool Dead
		{
			get
			{
				if (emitter.Alive) return false;
				if (paramParticleInfinite) return false;
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

			((PointEmitter)emitter).Point = Position;
			emitter.Update(time);

			var particlePosition = particleData.Get<Vector2>("Position");
			var particleVelocity = particleData.Get<Vector2>("Velocity");
			var particleLife = particleData.Get<float>("Life");
			
			for (int i = 0; i < particleData.ActiveParticles;)
			{
				particlePosition[i] += particleVelocity[i] * time.ElapsedSeconds;
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
			var quadBatch = renderContext.QuadBatch;
			
			// TODO: Figure out the best blending mode
			if (paramBlendMode == BlendMode.BlendmodeAlpha)
			{
				paramBlendMode = BlendMode.BlendmodeNonPremultiplied;
			}

			quadBatch.Begin(transform, paramBlendMode);
			
			var particlePosition = particleData.Get<Vector2>("Position");
			var particleLife = particleData.Get<float>("Life");

			for (int i = 0; i < particleData.ActiveParticles; i++)
			{
				var p = particlePosition[i];
				var a = particleAge[i];
				var l = particleLife[i];
				var lifeFraction = a / l;
				var color = paramParticleColor.Evaluate(lifeFraction);
				var s = paramParticleScale.Get(emitter.LifeFractional, lifeFraction);
				var sx = paramParticleScaleX.Get(emitter.LifeFractional, lifeFraction);
				var sy = paramParticleScaleY.Get(emitter.LifeFractional, lifeFraction);
				
				quadBatch.Draw(particleTexture, MathTools.Create2DAffineMatrix(p.X, p.Y, s * sx, s * sy, 0), color, particleTexture.Size / 2, 0);
			}
			
			quadBatch.End();

			var childTransform = transform * MathTools.Create2DAffineMatrix(Position.X, Position.Y, 1, 1, 0);
			foreach (var system in SubSystems)
			{
				system.Draw(renderContext, childTransform);
			}
		}
	}
}
