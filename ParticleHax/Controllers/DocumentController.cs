using System;

using MG.Framework.Utility;
using MG.ParticleEditorWindow;
using System.IO;

namespace MG.ParticleHax.Controllers
{
	class DocumentController
	{
		public const string ProjectFileExtension = ".pe";

		private MainController controller;
		private Model model;
		private const string projectFilters = "ParticleHax Projects (*" + ProjectFileExtension + ")|*" + ProjectFileExtension + "|All files (*.*)|*.*";

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
			var result = controller.ShowOpenDialog("Open ParticleHax Project...", projectFilters, "");
			if (result.Accepted)
			{
				Open(result.SelectedPath);
			}
		}

		public bool Open(FilePath file)
		{
			if (!Directory.Exists(file.ParentDirectory))
			{
				Log.Error("Tried to open file with non-existent folder: " + file.ParentDirectory);
				return false;
			}

			if (!Close()) return false;
			
			Environment.CurrentDirectory = file.ParentDirectory; // Tentatively set the current directory so that paths can be set correctly
			
			model.Clear();

			try
			{
				model.DefinitionTable.Load(file);
			}
			catch (Exception e)
			{
				controller.ShowMessage("<b>Error on load!</b>\n\nMessage: " + e.Message, MainWindow.MessageType.Error);
				Log.Error("- Error: " + e.Message);
				return false;
			}

			model.DocumentFile = file;
			controller.UpdateTree = true;
			controller.UpdateTitle = true;
			Log.Info("Opened project: " + file);

			OpenDocument.Invoke();
			return true;
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

				Log.Info("Closed project.");
			}

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
			var result = controller.ShowSaveDialog("Save ParticleHax Project...", projectFilters, model.DocumentFile);
			if (result.Accepted)
			{
				var outputFile = result.SelectedPath;
				if (!outputFile.HasExtension(ProjectFileExtension) && result.FilterIndex == 0)
				{
					outputFile = outputFile.ChangeExtension(ProjectFileExtension);
				}

				return Save(outputFile);
			}
			return false;
		}

		private bool Save(FilePath outputFile)
		{
			bool success = false;
			var oldCurrentDirectory = Environment.CurrentDirectory;

			try
			{
				Environment.CurrentDirectory = outputFile.ParentDirectory; // Tentatively set the current directory so that paths can be set correctly
				
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
				Environment.CurrentDirectory = oldCurrentDirectory;

				controller.ShowMessage("<b>Error on save!</b>\n\nMessage: " + e.Message, MainWindow.MessageType.Error);
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
