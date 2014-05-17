using System;
using System.Diagnostics;

using Gtk;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Assets;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class MainController : IDisposable
	{
		private MainWindow window;
		private Model model;
		private AssetHandler assetHandler;
		private DocumentController documentController;
		private RenderController renderController;
		private TreeController treeController;
		private PropertyController propertyController;

		private Stopwatch startStopwatch = new Stopwatch();
		private Stopwatch frameStopwatch = new Stopwatch();
		
		public MainController()
		{
			window = new MainWindow("Window");
			window.Closing += WindowOnClosing;
			window.Closed += WindowOnClosed;

			model = new Model();
			model.UndoHandler = new UndoHandler(1000);
			model.Declaration = new ParticleDeclarationTable();
			model.Declaration.Load("ParticleDeclarations.xml");
			
			model.Definition = new ParticleDefinitionTable();
			//model.Definition.Load("definitions.xml");
			
			assetHandler = new AssetHandler(".");

			documentController = new DocumentController(model, window);
			renderController = new RenderController(model, assetHandler, window.RenderView);
			treeController = new TreeController(model, window.TreeView);
			propertyController = new PropertyController(model, window.PropertyView);
			
			treeController.ItemSelected += renderController.OnItemSelected;
			treeController.ItemSelected += propertyController.OnItemSelected;
			documentController.NewDocument += treeController.OnNewDocument;
			documentController.New();

			Application.Update += Update;
			startStopwatch.Start();
		}

		private void Update()
		{
			float elapsedSeconds = 0;
			if (!frameStopwatch.IsRunning)
			{
				frameStopwatch.Start();
			}
			else
			{
				elapsedSeconds = (float)frameStopwatch.Elapsed.TotalSeconds;
				frameStopwatch.Restart();
			}

			assetHandler.Update();

			renderController.Update(new Time(elapsedSeconds, startStopwatch.Elapsed.TotalSeconds));

			//particleSystem.Update(new Time(elapsedTime, 0));

			//Gtk.Application.Invoke(
			//    (sender, args) =>
			//        {
			//            MainGL.QueueDraw();
			//            statusbar5.Pop(0);
			//            statusbar5.Push(0, "Particles: " + particleSystem.ActiveParticles.ToString());
			//        });

			//Thread.Sleep(10);

			//statusbar5.Pop(0);
			//statusbar5.Push(0, "Particles: " + particleSystem.ActiveParticles.ToString());

			//MainGL.QueueDraw();
			// again after the timeout period expires.   Returning false would
			// terminate the timeout.

			window.StatusText = model.StatusText;
			window.RenderView.Refresh();			
		}

		private void WindowOnClosed()
		{
			Application.Quit();
		}

		private void WindowOnClosing(MainWindow.ClosingEventArgs closingEventArgs)
		{
			closingEventArgs.Cancel = false;
		}

		public void Dispose()
		{
			window.Dispose();
		}
	}
}