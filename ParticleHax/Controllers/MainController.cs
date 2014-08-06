using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using MG.EditorCommon;
using MG.EditorCommon.FileAssociation;
using MG.EditorCommon.Undo;
using MG.Framework.Assets;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleHax.Controllers
{
	class MainController : IDisposable
	{
		private MainWindow window;
		private Model model;
		private AssetHandler assetHandler;
		private DocumentController documentController;
		private RenderController renderController;
		private TreeController treeController;
		private ParameterController parameterController;
		private SettingsController settingsController;
		private ClipboardController clipboardController;
		private bool updateBlocked;

		private Stopwatch startStopwatch = new Stopwatch();
		private Stopwatch frameStopwatch = new Stopwatch();
		
		public string StatusText;
		public bool UpdateTree;
		public bool UpdateTitle;
		public bool UpdateParticleSystem;
		public int SelectDefinition;
		public string SelectParameter;

		public DocumentController DocumentController { get { return documentController; } }

		public void ShowMessage(string message, MainWindow.MessageType messageType)
		{
			updateBlocked = true;
			window.ShowMessage(message, messageType);
			updateBlocked = false;
		}

		public MainWindow.ResponseType ShowMessageOkCancel(string message, MainWindow.MessageType messageType)
		{
			updateBlocked = true;
			var r = window.ShowMessageOkCancel(message, messageType);
			updateBlocked = false;
			return r;
		}

		public MainWindow.ResponseType ShowMessageYesNoCancel(string message, MainWindow.MessageType messageType)
		{
			updateBlocked = true;
			var r = window.ShowMessageYesNoCancel(message, messageType);
			updateBlocked = false;
			return r;
		}

		public MainWindow.DialogResult ShowSaveDialog(string title, string filters, FilePath startPath)
		{
			updateBlocked = true;
			var r = window.ShowSaveDialog(title, filters, startPath);
			updateBlocked = false;
			return r;
		}

		public MainWindow.DialogResult ShowOpenDialog(string title, string filters, FilePath startPath)
		{
			updateBlocked = true;
			var r = window.ShowOpenDialog(title, filters, startPath);
			updateBlocked = false;
			return r;
		}

		public MainController()
		{
			var binaryPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var binaryDir = Path.GetDirectoryName(binaryPath);
			Log.Info("Location: " + binaryPath);
			Log.Info("Save Location: " + Framework.Framework.SaveDataFolder);

			Environment.CurrentDirectory = binaryDir; // HACK: Set working directory to make Path.GetFullPath not crash on Mac.
			Log.Info("Current directory: " + Environment.CurrentDirectory);

			//if (!FileAssociation.IsAssociated(DocumentController.ProjectFileExtension))
			{
				var icon = binaryDir + "\\icon.ico";
				FileAssociation.Associate(DocumentController.ProjectFileExtension, "GameHax.ParticleHax", "ParticleHax Project", icon, binaryPath);
			}

			Log.Info("Creating window.");
			window = new MainWindow("");
			window.Closing += WindowOnClosing;
			window.Closed += WindowOnClosed;

			Log.Info("Creating asset handler. Directory: " + binaryDir);
			assetHandler = new AssetHandler(binaryDir);

			model = new Model();
			model.UndoHandler = new UndoHandler(1000);
			model.UndoHandler.AfterStateChanged += AfterUndo;
			model.ParticleEffectPool = new ParticleEffectPool(assetHandler);
			model.DeclarationTable = new ParticleDeclarationTable();
			model.DeclarationTable.Load(assetHandler.GetFullPath("ParticleDeclarations.xml"));
			model.DefinitionTable = new ParticleDefinitionTable();
			
			documentController = new DocumentController(this, model);
			renderController = new RenderController(this, model, assetHandler, window.RenderView);
			treeController = new TreeController(this, model, window.TreeView);
			parameterController = new ParameterController(this, model, window.ParameterView);
			settingsController = new SettingsController(window);
			clipboardController = new ClipboardController(this, model, window.Clipboard);
			
			window.FileNew += documentController.New;
			window.FileOpen += documentController.Open;
			window.FileClose += () => documentController.Close();
			window.FileSave += () => documentController.Save();
			window.FileSaveAs += () => documentController.SaveAs();
			window.EditUndo += documentController.Undo;
			window.EditRedo += documentController.Redo;
			window.EditCut += () => clipboardController.Cut(model.CurrentDefinitionId);
			window.EditCopy += () => clipboardController.Copy(model.CurrentDefinitionId);
			window.EditPaste += () => clipboardController.Paste(model.CurrentDefinitionId);
			treeController.ItemSelected += renderController.OnItemSelected;
			treeController.ItemSelected += parameterController.OnChangeDefinition;
			treeController.ItemSelected += OnItemSelected;
			treeController.ItemCut += id => clipboardController.Cut(id);
			treeController.ItemCopy += id => clipboardController.Copy(id);
			treeController.ItemPaste += id => clipboardController.Paste(id);

			documentController.NewDocument += treeController.OnNewDocument;
			documentController.OpenDocument += treeController.OnOpenDocument;
			documentController.BeforeSaveDocument += parameterController.CancelEdit;

			AfterUndo();

			Application.Update += Update;
			startStopwatch.Start();

			Log.Info("Done creating MainController.");
		}
		
		private void Update()
		{
			if (updateBlocked) return;
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
				if (model.ParticleEffect != null)
				{
					model.CurrentDefinition.ReloadCache();
					model.ParticleEffect.Reload();
				}		
			}

			if (SelectDefinition != 0)
			{
				treeController.SelectItem(SelectDefinition);
				SelectDefinition = 0;
			}

			if (SelectParameter != null)
			{
				parameterController.SelectParameter(SelectParameter);
				SelectParameter = null;
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

		private void OnItemSelected(ParticleDefinition particleDefinition)
		{
			window.CutCopyEnabled = particleDefinition != null;
		}

		private void UpdateTitleInternal()
		{
			var title = new StringBuilder();
			title.Append("ParticleHax");

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