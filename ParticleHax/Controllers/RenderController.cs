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

		public bool Loaded { get { return loaded; } }

		public RenderController(MainController controller, Model model, AssetHandler assetHandler, RenderView renderView)
		{
			this.controller = controller;
			this.model = model;
			this.assetHandler = assetHandler;

			renderView.Load += Load;
			renderView.Draw += Draw;
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

					controller.StatusText = "Particle system dead.";
				}
				else
				{
					controller.StatusText = "Particles: " + particleSystem.ActiveParticles.ToString();
				}
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

				Log.Info("Creating particle system from definition: " + definition.Name);
				model.ParticleSystem = model.ParticleSystemPool.Create(definition);
				UpdateParticleSystemPosition();
			}
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
		}

		private void UpdateParticleSystemPosition()
		{
			var particleSystem = model.ParticleSystem;
			if (particleSystem == null) return;
			
			particleSystem.Position = new Vector2(Screen.PrimaryScreen.NormalizedScreenArea.Center);
		}

		//private void ResetChildPosition(ParticleSystem parent)
		//{
		//    foreach (var child in parent.SubSystems)
		//    {
				
		//    }
		//}
	}
}
