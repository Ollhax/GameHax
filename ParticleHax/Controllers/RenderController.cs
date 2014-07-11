using System;

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

		public void Update(Time time)
		{
			var particleSystem = model.ParticleSystem;
			if (particleSystem != null)
			{
				particleSystem.Update(time);
				
				if (particleSystem.Dead)
				{
					model.ParticleSystemPool.Destroy(particleSystem);
					model.ParticleSystem = null;
				}
				else
				{
					controller.StatusText = "Particles: " + particleSystem.ActiveParticles.ToString();
				}
			}

			if (particleSystem == null || lastViewMode != ViewMode)
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
				model.ParticleSystem = null;
				return;
			}

			var viewMode = ViewMode;
			lastViewMode = viewMode;
			
			if (viewMode == ParticleView.FullTree)
			{
				while (definition.Parent != null)
					definition = definition.Parent;
			}

			if (model.ParticleSystem == null || definition != model.ParticleSystem.Definition)
			{
				if (model.ParticleSystem != null)
				{
					model.ParticleSystemPool.Destroy(model.ParticleSystem);
				}

				if (!Disabled(definition))
				{
					if (model.ParticleSystem != null && definition != model.ParticleSystem.Definition)
					{
						particlePosition = null;
					}

					//Log.Info("Creating particle system from definition: " + definition.Name);
					model.ParticleSystem = model.ParticleSystemPool.Create(definition);
					model.ParticleSystem.SetGravityRecursive(new Vector2(0, 100));
					
					UpdateParticleSystemPosition();
				}
				else
				{
					controller.StatusText = "Particle system disabled (emitter life = 0)";
				}
			}
		}
		
		private bool Disabled(ParticleDefinition definition)
		{
			return definition.Parameters["EmitterLife"].Value.Get<float>() <= 0;
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

			var particleSystem = model.ParticleSystem;
			if (particleSystem != null)
			{
				UpdateParticleSystemPosition();
				DrawEffect(renderContext, particleSystem, ViewMode != ParticleView.Selected);
			}

			if (Settings.Get<bool>("Crosshair.Enable") && (particlePosition != null || particleSystem != null))
			{
				var center = particlePosition ?? particleSystem.Position;
				var length = 15.0f;
				var color = Settings.Get<Color>("Crosshair.Color");

				renderContext.PrimitiveBatch.Begin(Matrix.Identity, BlendMode.BlendmodeNonPremultiplied);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(length, 0), center + new Vector2(length, 0)), color);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(0, length), center + new Vector2(0, length)), color);
				renderContext.PrimitiveBatch.End();
			}
		}

		private void DrawEffect(RenderContext renderContext, ParticleSystem particleSystem, bool drawChildren)
		{
			if (drawChildren)
			{
				particleSystem.Draw(renderContext, Matrix.Identity);
			}
			else
			{
				particleSystem.DrawCurrent(renderContext, Matrix.Identity);
			}
		}

		private void UpdateParticleSystemPosition()
		{
			var particleSystem = model.ParticleSystem;
			if (particleSystem == null) return;
			
			particleSystem.SetPositionRecursive(new Vector2(particlePosition ?? Screen.PrimaryScreen.NormalizedScreenArea.Center));
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
