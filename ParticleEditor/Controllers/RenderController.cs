using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class RenderController
	{
		private Model model;
		private AssetHandler assetHandler;
		private bool loaded;

		public RenderController(Model model, AssetHandler assetHandler, RenderView renderView)
		{
			this.model = model;
			this.assetHandler = assetHandler;

			renderView.Load += Load;
			renderView.Draw += Draw;
		}
		
		private void Load()
		{
			loaded = true;
		}

		public void Update(Time time)
		{
			var particleSystem = model.ParticleSystem;
			if (particleSystem != null)
			{
				particleSystem.Update(time);
				model.StatusText = "Particles: " + particleSystem.ActiveParticles.ToString();
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
				model.ParticleSystem = new ParticleSystem(assetHandler, definition);
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
	}
}
