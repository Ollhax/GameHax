using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Assets;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditor.Actions;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class TreeController
	{
		private MainController controller;
		private Model model;
		private TreeView treeView;

		public event Action<ParticleDefinition> ItemSelected = delegate { };
		
		public TreeController(MainController controller, Model model, TreeView treeView)
		{
			this.controller = controller;
			this.model = model;
			this.treeView = treeView;
			
			treeView.ItemSelected += OnItemSelected;
			treeView.ItemRenamed += OnItemRenamed;
			treeView.CreateContextMenu += OnCreateContextMenu;
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}
		
		public void CreateEntry(string declarationName)
		{
			var particleDefinition = CreateParticle(declarationName);
			model.Definition.Definitions.Add(particleDefinition.Name, particleDefinition);

			UpdateTree();
		}

		public void UpdateTree()
		{
			var items = new List<KeyValuePair<int, string>>();
			foreach (var def in model.Definition.Definitions)
			{
				items.Add(new KeyValuePair<int, string>(def.Value.InternalId, def.Value.Name));
			}

			treeView.SetValues(items);
		}
		
		private void OnUndoRedoEvent(IUndoableAction undoableAction)
		{
			var action = undoableAction;
			var undoableActionGroup = undoableAction as UndoableActionGroup;
			if (undoableActionGroup != null && undoableActionGroup.Actions.Count > 0)
			{
				action = undoableActionGroup.Actions[undoableActionGroup.Actions.Count - 1];
			}

			var particleAction = action as UndoableParticleAction;
			if (particleAction != null)
			{
				treeView.SelectItem(particleAction.CurrentDefinitionId, true);
			}
		}

		public void OnNewDocument()
		{
			model.DefinitionIdCounter = 1;
			
			if (model.Declaration.DeclarationsList.Count > 0)
			{
				var decl = model.Declaration.DeclarationsList[0];

				for (int i = 0; i < 20; i++)
				{
					CreateEntry(decl.Name);
				}
			}
		}

		private ParticleDefinition CreateParticle(string name)
		{
			ParticleDeclaration declaration;
			if (!model.Declaration.Declarations.TryGetValue(name, out declaration)) return null;
			
			var definition = new ParticleDefinition();
			definition.InternalId = model.DefinitionIdCounter++;
			definition.Name = declaration.Name + definition.InternalId;
			definition.Declaration = name;

			foreach (var declarationParameterPair in declaration.Parameters)
			{
				var declarationParameter = declarationParameterPair.Value;
				var definitionParameter = new ParticleDefinition.Parameter();

				definitionParameter.Name = declarationParameter.Name;
				definitionParameter.Value = new Any(declarationParameter.DefaultValue);
				definitionParameter.Random = new Any(declarationParameter.DefaultValueRandom);

				definition.Parameters.Add(definitionParameter.Name, definitionParameter);
			}
			
			return definition;
		}

		private void OnItemSelected(int id)
		{
			var def = model.GetDefinitionById(id);
			model.CurrentDefinition = def;
			ItemSelected(def);
		}
		
		private bool OnItemRenamed(int id, string newText)
		{
			var def = model.GetDefinitionById(id);
			if (def != null)
			{
				var renameAction = new RenameAction(controller, model, id, newText);
				if (!model.UndoHandler.ExecuteAction(renameAction) && !string.IsNullOrEmpty(renameAction.Error))
				{
					controller.ShowMessage(renameAction.Error, MainWindow.MessageType.Error);
				}
			}
			return false;
		}

		private void OnCreateContextMenu(TreeView.ContextMenu contextMenu)
		{
			var def = model.GetDefinitionById(contextMenu.ItemId);

			var declarations = model.Declaration.DeclarationsList;
			if (declarations.Count > 0)
			{
				var addEntry = new TreeView.ContextMenu.Entry("Add...", null);
				addEntry.SubEntries = new List<TreeView.ContextMenu.Entry>();

				foreach (var decl in declarations)
				{
					var d = decl;
					addEntry.SubEntries.Add(new TreeView.ContextMenu.Entry(decl.Name, () => OnContextMenuAdd(d.Name)));
				}

				contextMenu.Entries.Add(addEntry);
			}
			
			if (def != null)
			{
				contextMenu.Entries.Add(new TreeView.ContextMenu.Entry("Remove \"" + def.Name + "\"", () => OnContextMenuRemove(def.InternalId)));
			}
		}

		private void OnContextMenuAdd(string declaration)
		{
			Log.P("Add " + declaration);
		}

		private void OnContextMenuRemove(int id)
		{
			Log.P("Remove " + id);
		}
	}
}
