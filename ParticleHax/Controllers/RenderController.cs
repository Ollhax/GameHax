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
		private MainController controller;
		private Model model;
		private AssetHandler assetHandler;
		private bool loaded;
		private Vector2? particlePosition;
		
		public bool Loaded { get { return loaded; } }

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

			if (particleSystem == null)
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

			if (model.ParticleSystem == null || definition != model.ParticleSystem.Definition)
			{
				if (model.ParticleSystem != null)
				{
					model.ParticleSystemPool.Destroy(model.ParticleSystem);
				}

				if (!Disabled(definition))
				{
					//Log.Info("Creating particle system from definition: " + definition.Name);
					model.ParticleSystem = model.ParticleSystemPool.Create(definition);
					model.ParticleSystem.Gravity = new Vector2(0, 100);
					particlePosition = null;
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
		
		private void Draw(RenderContext renderContext)
		{
			GraphicsDevice.ClearColor = Color.CornflowerBlue;
			GraphicsDevice.Clear();
			GraphicsDevice.SetViewport((Rectangle)renderContext.ActiveScreen.NormalizedScreenArea, renderContext.ActiveScreen);
			
			var particleSystem = model.ParticleSystem;
			if (particleSystem != null)
			{
				UpdateParticleSystemPosition();
				particleSystem.Draw(renderContext);
			}

			if (Settings.Get<bool>("Crosshair.Enable"))
			{
				var center = renderContext.ActiveScreen.NormalizedScreenArea.Center;
				var length = 15.0f;
				var color = Settings.Get<Color>("Crosshair.Color");

				renderContext.PrimitiveBatch.Begin(Matrix.Identity, BlendMode.BlendmodeNonPremultiplied);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(length, 0), center + new Vector2(length, 0)), color);
				renderContext.PrimitiveBatch.Draw(new Line(center - new Vector2(0, length), center + new Vector2(0, length)), color);
				renderContext.PrimitiveBatch.End();
			}
		}

		private void UpdateParticleSystemPosition()
		{
			var particleSystem = model.ParticleSystem;
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
