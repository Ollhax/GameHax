using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleHax.Controllers
{
	class RenderController
	{
		public enum ParticleView
		{
			Selected,
			SubTree,
			FullTree
		}

		public enum ParticleQualityLevel
		{
			Low,
			Medium,
			High
		}

		private MainController controller;
		private Model model;
		private AssetHandler assetHandler;
		private bool loaded;
		private Vector2? particlePosition;
		private ParticleView lastViewMode = ParticleView.FullTree;
		
		public bool Loaded { get { return loaded; } }
		
		public ParticleView ViewMode
		{
			get { return (ParticleView)Settings.Get<int>("ViewMode"); }
		}

		public ParticleQualityLevel QualityLevel
		{
			get { return (ParticleQualityLevel)Settings.Get<int>("QualityLevel"); }
		}

		public RenderController(MainController controller, Model model, AssetHandler assetHandler, RenderView renderView)
		{
			this.controller = controller;
			this.model = model;
			this.assetHandler = assetHandler;

			renderView.Load += Load;
			renderView.Draw += Draw;
			renderView.LeftMousePress += OnPress;
		}
		
		private void Load()
		{
			Log.Info("Done loading render view.");
			loaded = true;
		}

		private int GetParticles(ParticleEffect effect)
		{
			int particles = effect.ParticleData.ActiveParticles;
			foreach (var subEffect in effect.SubSystems)
			{
				particles += GetParticles(subEffect);
			}

			return particles;
		}

		public void Update(Time time)
		{
			ParticleDefinition.GlobalQualityLevel = (int)QualityLevel;

			var particleEffect = model.ParticleEffect;
			if (particleEffect != null)
			{
				ParticleSimulator.Update(particleEffect, time);
				
				if (particleEffect.Dead)
				{
					model.ParticleEffectPool.Destroy(particleEffect);
					model.ParticleEffect = null;
				}
				else
				{
					controller.StatusText = "Particles: " + GetParticles(particleEffect).ToString();
				}
			}

			if (particleEffect == null || lastViewMode != ViewMode)
			{
				OnItemSelected(model.CurrentDefinition);
			}
		}

		public void OnItemSelected(ParticleDefinition definition)
		{
			if (!loaded)
			{
				Log.Warning("Got item selection before gl widget was loaded.");
				return;
			}

			if (definition == null)
			{
				model.ParticleEffect = null;
				return;
			}

			var viewMode = ViewMode;
			lastViewMode = viewMode;
			
			if (viewMode == ParticleView.FullTree)
			{
				while (definition.Parent != null)
					definition = definition.Parent;
			}

			if (model.ParticleEffect == null || definition != model.ParticleEffect.Definition)
			{
				if (model.ParticleEffect != null)
				{
					model.ParticleEffectPool.Destroy(model.ParticleEffect);
					model.ParticleEffect = null;
				}

				if (!Disabled(definition))
				{
					if (model.ParticleEffect != null && definition != model.ParticleEffect.Definition)
					{
						particlePosition = null;
					}

					//Log.Info("Creating particle system from definition: " + definition.Name);
					model.ParticleEffect = model.ParticleEffectPool.Create(definition);
					model.ParticleEffect.Gravity = new Vector2(0, 1);
					
					UpdateParticleSystemPosition();
				}
				//else
				//{
				//    //controller.StatusText = "Particle system disabled (emitter life = 0)";
				//    model.ParticleEffect = null;
				//}
			}
		}

		public void Restart()
		{
			if (model.ParticleEffect != null)
			{
				model.ParticleEffectPool.Destroy(model.ParticleEffect);
				model.ParticleEffect = null;
			}
		}
		
		private bool Disabled(ParticleDefinition definition)
		{
			return IsOnlyGroups(definition);// definition.Declaration == "Group";// || definition.Parameters["EmitterLife"].Value.Get<float>() <= 0;
		}

		private bool IsOnlyGroups(ParticleDefinition definition)
		{
			if (!definition.IsGroup) return false;
			foreach (var child in definition.Children)
			{
				if (!IsOnlyGroups(child)) return false;
			}

			return true;
		}

		private Color GetBackgroundColor()
		{
			int index = Settings.Get<int>("Background.Current");
			return Settings.Get<Color>("Background.Color" + ((index + 1) % 10));
		}

		private void DrawCheckboard(RenderContext renderContext, Color color)
		{
			var colorWhite = Color.White;
			var colorBlack = new Color(230, 230, 230, 255);
			int size = 60;
			var area = renderContext.ActiveScreen.NormalizedScreenArea;
			var primitiveBatch = renderContext.PrimitiveBatch;

			primitiveBatch.Begin();
			for (int x = 0; x < area.Width / size; x++)
			{
				for (int y = 0; y < area.Height / size; y++)
				{
					bool even = (x % 2 == 0) ^ (y % 2 == 0);

					primitiveBatch.DrawFilled(new RectangleF(area.X + x * size, area.Y + y * size - 1, size + 1, size + 1), even ? colorWhite : colorBlack);
				}
			}

			if (color.A != 0)
			{
				primitiveBatch.DrawFilled(area, color);
			}
			
			primitiveBatch.End();
		}

		private void Draw(RenderContext renderContext)
		{
			var clearColor = GetBackgroundColor();
			GraphicsDevice.ClearColor = clearColor;
			GraphicsDevice.Clear();
			GraphicsDevice.SetViewport((Rectangle)renderContext.ActiveScreen.NormalizedScreenArea, renderContext.ActiveScreen);

			if (clearColor.A != 255)
			{
				DrawCheckboard(renderContext, clearColor);
			}

			var particleEffect = model.ParticleEffect;
			if (particleEffect != null)
			{
				UpdateParticleSystemPosition();
				DrawEffect(renderContext, particleEffect, ViewMode != ParticleView.Selected);
			}

			if (Settings.Get<bool>("Crosshair.Enable") && (particlePosition != null || particleEffect != null))
			{
				var center = particlePosition ?? particleEffect.Position;
				var length = 15.0f;
				var color = Settings.Get<Color>("Crosshair.Color");

				renderContext.PrimitiveBatch.Begin(Matrix.Identity, BlendMode.BlendmodeNonPremultiplied);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(length, 0), center + new Vector2(length, 0)), color);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(0, length), center + new Vector2(0, length)), color);
				renderContext.PrimitiveBatch.End();
			}
		}

		private void DrawEffect(RenderContext renderContext, ParticleEffect particleEffect, bool drawChildren)
		{
			if (model.InvisibleIds.Count != 0)
			{
				if (!model.InvisibleIds.Contains(particleEffect.Definition.Id))
				{
					ParticleVisualizer.DrawCurrent(particleEffect, renderContext, Matrix.Identity);
				}
				if (drawChildren)
				{
					for (int i = particleEffect.SubSystems.Count - 1; i >= 0; i--)
					{
						var system = particleEffect.SubSystems[i];
						DrawEffect(renderContext, system, drawChildren);
					}
				}
			}
			else if (drawChildren)
			{
				ParticleVisualizer.Draw(particleEffect, renderContext, Matrix.Identity);
			}
			else
			{
				ParticleVisualizer.DrawCurrent(particleEffect, renderContext, Matrix.Identity);
			}
		}

		private void UpdateParticleSystemPosition()
		{
			var particleSystem = model.ParticleEffect;
			if (particleSystem == null) return;
			
			particleSystem.Position = new Vector2(particlePosition ?? Screen.PrimaryScreen.NormalizedScreenArea.Center);
		}

		private void OnPress(Vector2 pos)
		{
			particlePosition = pos;
		}

		//private void ResetChildPosition(ParticleSystem parent)
		//{
		//    foreach (var child in parent.SubSystems)
		//    {
				
		//    }
		//}
	}
}
