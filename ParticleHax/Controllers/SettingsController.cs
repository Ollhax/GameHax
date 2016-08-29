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
			
			float step = 1.0f / 6.0f;
			float v = 0;
			Settings.Set("Background.Current", 0);

			Settings.Set("Background.Color1", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color2", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color3", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color4", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color5", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color6", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color7", new Color(v, v, v, 1.0f)); v += step;
			Settings.Set("Background.Color8", Color.DarkViolet);
			Settings.Set("Background.Color9", Color.CornflowerBlue);
			Settings.Set("Background.Color0", Color.Transparent);

			Settings.Set("Crosshair.Enable", true);
			Settings.Set("Crosshair.Color", new Color(1.0f, 1.0f, 1.0f, 0.5f));

			Settings.Set("ShowGrid", false);
			Settings.Set("GridSize", 120);

			Settings.Set("ViewMode", (int)(RenderController.ParticleView.FullTree));
			Settings.Set("QualityLevel", (int)(RenderController.ParticleQualityLevel.High));
			
			Settings.Load(); // Override defaults
			Settings.Save(); // Save any missing settings

			window.ViewShowOrigin = Settings.Get<bool>("Crosshair.Enable");
			window.ToggleShowOrigin += OnToggleShowOrigin;

			window.ViewShowGrid = Settings.Get<bool>("ShowGrid");
			window.ToggleShowGrid += OnToggleShowGrid;

			window.ViewGridSize = Settings.Get<int>("GridSize");
			window.SetGridSize += OnSetGridSize;

			window.ViewMode = Settings.Get<int>("ViewMode");
			window.ViewModeChanged += OnViewModeChanged;

			window.QualityLevel = Settings.Get<int>("QualityLevel");
			window.QualityLevelChanged += OnQualityLevelChanged;

			window.CurrentBackgroundColorIndex = Settings.Get<int>("Background.Current");
			window.BackgroundColorChanged += OnBackgroundColorChanged;

			for (int i = 0; i < 10; i++)
			{
				int index = (i + 1) % 10;
				window.SetBackgroundColor(i, Settings.Get<Color>("Background.Color" + index));
			}
		}
		
		private void OnToggleShowOrigin()
		{
			Settings.Set("Crosshair.Enable", window.ViewShowOrigin);
			Settings.Save();
		}

		private void OnToggleShowGrid()
		{
			Settings.Set("ShowGrid", window.ViewShowGrid);
			Settings.Save();
		}

		private void OnSetGridSize()
		{
			Settings.Set("GridSize", window.ViewGridSize);
			Settings.Save();
		}

		private void OnViewModeChanged()
		{
			Settings.Set("ViewMode", window.ViewMode);
			Settings.Save();
		}

		private void OnQualityLevelChanged()
		{
			Settings.Set("QualityLevel", window.QualityLevel);
			Settings.Save();
		}

		private void OnBackgroundColorChanged()
		{
			Settings.Set("Background.Current", window.CurrentBackgroundColorIndex);
			Settings.Save();
		}
	}
}
