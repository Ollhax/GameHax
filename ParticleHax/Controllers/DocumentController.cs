using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;
using System.IO;

using MG.ParticleHax.Actions;

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
		public event Action BeforeSaveDocument = delegate { };
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
			
			model.Clear();

			try
			{
				model.DefinitionTable.Load(file);

				// Convert path parameters
				ToAbsolutePath(file.ParentDirectory);
				
				// Add missing parameters
				foreach (var defPair in model.DefinitionTable.Definitions)
				{
					ParticleDeclaration declaration;
					if (model.DeclarationTable.Declarations.TryGetValue(defPair.Declaration, out declaration))
					{
						model.Modified |= AddAction.AddMissingParameters(declaration.Parameters, defPair.Parameters, true);
					}
					else
					{
						Log.Warning("Could not find declation: " + declaration);
					}
				}

				Environment.CurrentDirectory = file.ParentDirectory;
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
			BeforeSaveDocument.Invoke();

			bool success = false;
			ToRelativePath(outputFile.ParentDirectory);

			try
			{
				model.DefinitionTable.Save(outputFile);
				model.DocumentFile = outputFile;
				model.Modified = false;
				controller.UpdateTitle = true;
				success = true;

				Log.Info("Saved to file " + outputFile);
				Environment.CurrentDirectory = outputFile.ParentDirectory;
			}
			catch (Exception e)
			{
				controller.ShowMessage("<b>Error on save!</b>\n\nMessage: " + e.Message, MainWindow.MessageType.Error);
				Log.Error("- Error: " + e.Message);
			}

			ToAbsolutePath(Environment.CurrentDirectory);

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

		private void ToAbsolutePath(string directory)
		{
			foreach (var d in model.DefinitionTable.Definitions)
			{
				ToAbsolutePath(directory, d.Parameters);
			}
		}

		private void ToRelativePath(string directory)
		{
			foreach (var d in model.DefinitionTable.Definitions)
			{
				ToRelativePath(directory, d.Parameters);
			}
		}

		private void ToAbsolutePath(string directory, Dictionary<string, ParticleDefinition.Parameter> parameters)
		{
			foreach (var param in parameters)
			{
				var v = param.Value.Value;
				if (v.IsFilePath()) // Paths are saved in relative format, convert to absolute format internally
				{
					v.Set((FilePath)v.Get<FilePath>().ToAbsolute(directory));
				}

				ToAbsolutePath(directory, param.Value.Parameters);
			}
		}

		private void ToRelativePath(string directory, Dictionary<string, ParticleDefinition.Parameter> parameters)
		{
			foreach (var param in parameters)
			{
				var v = param.Value.Value;
				
				if (v.IsFilePath()) // Save relative paths
				{
					v.Set((FilePath)v.Get<FilePath>().ToRelative(directory).ToString().Replace('\\', '/')); // Don't want constant changes depending on platform
				}

				ToRelativePath(directory, param.Value.Parameters);
			}
		}
	}
}
