using System;

using MG.EditorCommon;
using MG.Framework.Numerics;
using MG.ParticleEditorWindow;

namespace MG.ParticleHax.Controllers
{
	class SettingsController
	{
		private MainWindow window;

		public SettingsController(MainWindow window)
		{
			this.window = window;

			Settings.Set("Background.Color", Color.Red);
			Settings.Set("Crosshair.Enable", false);
			Settings.Set("Crosshair.Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));
			
			Settings.Load(); // Override defaults
			Settings.Save(); // Save any missing settings

			window.ViewShowOrigin = Settings.Get<bool>("Crosshair.Enable");
			window.ToggleShowOrigin += OnToggleShowOrigin;
		}

		private void OnToggleShowOrigin()
		{
			Settings.Set("Crosshair.Enable", window.ViewShowOrigin);
			Settings.Save();
		}
	}
}
