using System;
using System.Diagnostics;
using System.Text;

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
		private MainParameterController mainParameterController;
		private InfoController infoController;
		private GraphController graphController;

		private Stopwatch startStopwatch = new Stopwatch();
		private Stopwatch frameStopwatch = new Stopwatch();
		
		public string StatusText;
		public bool UpdateTree;
		public bool UpdateTitle;
		public bool UpdateParticleSystem;
		public int SelectDefinition;
		public string SelectParameter;
		public string SelectSubParameter;

		public void ShowMessage(string message, MainWindow.MessageType messageType)
		{
			window.ShowMessage(message, messageType);
		}

		public MainWindow.ResponseType ShowMessageOkCancel(string message, MainWindow.MessageType messageType)
		{
			return window.ShowMessageOkCancel(message, messageType);
		}

		public MainWindow.ResponseType ShowMessageYesNoCancel(string message, MainWindow.MessageType messageType)
		{
			return window.ShowMessageYesNoCancel(message, messageType);
		}

		public MainWindow.DialogResult ShowSaveDialog(string title, string filters, FilePath startPath)
		{
			return window.ShowSaveDialog(title, filters, startPath);
		}

		public MainWindow.DialogResult ShowOpenDialog(string title, string filters, FilePath startPath)
		{
			return window.ShowOpenDialog(title, filters, startPath);
		}

		public MainController()
		{
			window = new MainWindow("");
			window.Closing += WindowOnClosing;
			window.Closed += WindowOnClosed;

			assetHandler = new AssetHandler(".");

			model = new Model();
			model.UndoHandler = new UndoHandler(1000);
			model.UndoHandler.AfterStateChanged += AfterUndo;
			model.ParticleManager = new ParticleManager(assetHandler);
			model.DeclarationTable = new ParticleDeclarationTable();
			model.DeclarationTable.Load("ParticleDeclarations.xml");
			model.DefinitionTable = new ParticleDefinitionTable();
			
			documentController = new DocumentController(this, model);
			renderController = new RenderController(this, model, assetHandler, window.RenderView);
			treeController = new TreeController(this, model, window.TreeView);
			mainParameterController = new MainParameterController(this, model, window.PropertyView);
			infoController = new InfoController(this, model, window.InfoView);
			graphController = new GraphController(this, model, window.GraphView);

			window.FileNew += documentController.New;
			window.FileOpen += documentController.Open;
			window.FileClose += () => documentController.Close();
			window.FileSave += () => documentController.Save();
			window.FileSaveAs += () => documentController.SaveAs();
			window.EditUndo += documentController.Undo;
			window.EditRedo += documentController.Redo;
			treeController.ItemSelected += renderController.OnItemSelected;
			treeController.ItemSelected += mainParameterController.OnChangeDefinition;
			mainParameterController.ParameterSelected += s => infoController.UpdateInfo();
			//mainParameterController.ParameterSelected += s => graphController.OnParameterSelected();
			infoController.SubParameterController.ParameterSelected += graphController.OnSubParameterSelected;
			infoController.SubParameterController.StartedEdit += mainParameterController.CancelEdit;
			mainParameterController.StartedEdit += infoController.SubParameterController.CancelEdit;

			graphController.GraphChanged += infoController.SubParameterController.OnChangeParameter;

			documentController.NewDocument += treeController.OnNewDocument;
			documentController.OpenDocument += treeController.OnOpenDocument;
			documentController.New();

			AfterUndo();

			Application.Update += Update;
			startStopwatch.Start();
		}
		
		private void Update()
		{
			if (!renderController.Loaded) return;

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

			if (UpdateTree)
			{
				UpdateTree = false;
				treeController.UpdateTree();
			}

			if (UpdateTitle)
			{
				UpdateTitle = false;
				UpdateTitleInternal();
			}

			if (UpdateParticleSystem)
			{
				UpdateParticleSystem = false;
				if (model.ParticleSystem != null)
				{
					model.CurrentDefinition.ReloadCache();
					model.ParticleSystem.Reload();
				}		
			}

			if (SelectDefinition != 0)
			{
				treeController.SelectItem(SelectDefinition);
				SelectDefinition = 0;
			}

			if (SelectParameter != null)
			{
				mainParameterController.SelectParameter(SelectParameter);
				//treeController.SelectParameter(SelectParameter);
				//infoController.
				//infoController.SetInfo(SelectParameter);
				SelectParameter = null;
			}

			if (SelectSubParameter != null)
			{
				//infoController.SubParameterController.SelectParameter(SelectSubParameter);
				SelectSubParameter = null;
			}

			var sensitive = window.Sensitive;
			if (model.DocumentOpen != sensitive)
			{
				window.Sensitive = model.DocumentOpen;
			}

			window.StatusText = StatusText;
			window.RenderView.Refresh();
		}

		private void WindowOnClosed()
		{
			Application.Quit();
		}

		private void WindowOnClosing(MainWindow.ClosingEventArgs closingEventArgs)
		{
			if (!documentController.Close())
			{
				closingEventArgs.Cancel = true;
			}
		}

		private void AfterUndo()
		{
			window.UndoEnabled = model.UndoHandler.UndoSteps > 0;
			window.RedoEnabled = model.UndoHandler.RedoSteps > 0;
			UpdateTitleInternal();
		}

		private void UpdateTitleInternal()
		{
			var title = new StringBuilder();
			title.Append("Particle Editor");

			if (model.DocumentOpen)
			{
				var documentName = "untitled project";
				if (!model.DocumentFile.IsNullOrEmpty)
				{
					documentName = model.DocumentFile.FileName;
				}

				title.Append(" - ");
				title.Append(documentName);

				if (model.Modified)
				{
					title.Append("*");
				}
			}

			window.Title = title.ToString();
		}

		public void Dispose()
		{
			window.Dispose();
		}
	}
}