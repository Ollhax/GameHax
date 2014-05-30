using System;

using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class DocumentController
	{
		private MainController controller;
		private Model model;
		private const string projectExtension = ".peproj";
		private const string projectFilters = "Particle Editor Projects (*" + projectExtension + ")|*" + projectExtension + "|All files (*.*)|*.*";

		public event Action NewDocument = delegate { };
		public event Action OpenDocument = delegate { };
		public event Action CloseDocument = delegate { };
		
		public DocumentController(MainController controller, Model model)
		{
			this.controller = controller;
			this.model = model;
		}

		public void New()
		{
			if (!Close()) return;

			Log.Info("New project created.");
			controller.UpdateTitle = true;
			NewDocument.Invoke();
		}

		public void Open()
		{
			var result = controller.ShowOpenDialog("Open Particle Editor Project...", projectFilters, "");
			if (result.Accepted)
			{
				if (!Close()) return;

				model.Clear();
				model.DefinitionTable.Load(result.SelectedPath);
				model.DocumentFile = result.SelectedPath;
				controller.UpdateTree = true;
				controller.UpdateTitle = true;
				Log.Info("Opened project: " + result.SelectedPath);

				OpenDocument.Invoke();
			}
		}

		public bool Close()
		{
			if (model.Modified)
			{
				var response = controller.ShowMessageYesNoCancel("This project contains unsaved changes. Do you want to save before you continue?", MainWindow.MessageType.Question);

				switch (response)
				{
					case MainWindow.ResponseType.Cancel:
					{
						return false;
					}

					case MainWindow.ResponseType.Yes:
					{
						if (!Save()) return false;
					}
					break;
				}
			}

			Log.Info("Closed project.");
			model.Clear();
			controller.UpdateTree = true;
			controller.UpdateTitle = true;
			CloseDocument.Invoke();

			return true;
		}

		public bool Save()
		{
			if (model.DocumentFile.IsNullOrEmpty)
			{
				return SaveAs();
			}
			
			return Save(model.DocumentFile);
		}

		public bool SaveAs()
		{
			var result = controller.ShowSaveDialog("Save Particle Editor Project...", projectFilters, model.DocumentFile);
			if (result.Accepted)
			{
				var outputFile = result.SelectedPath;
				if (!outputFile.HasExtension(projectExtension) && result.FilterIndex == 0)
				{
					outputFile = outputFile.ChangeExtension(projectExtension);
				}

				return Save(outputFile);
			}
			return false;
		}

		private bool Save(FilePath outputFile)
		{
			bool success = false;

			try
			{
				model.DefinitionTable.Save(outputFile);
				model.DocumentFile = outputFile;
				model.Modified = false;
				controller.UpdateTitle = true;
				controller.UpdateTree = true;
				success = true;

				Log.Info("Saved to file " + outputFile);
			}
			catch (Exception e)
			{
				controller.ShowMessage("<b>Error on save!</b>\n\nException: " + e.Message, MainWindow.MessageType.Error);
				Log.Error("- Error: " + e.Message);
			}
			
			return success;
		}

		public void Undo()
		{
			model.UndoHandler.Undo();
		}

		public void Redo()
		{
			model.UndoHandler.Redo();
		}
	}
}
