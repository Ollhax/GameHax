using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleHax.Actions;
using MG.ParticleEditorWindow;

namespace MG.ParticleHax.Controllers
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
			treeView.ItemMoved += OnItemMoved;
			treeView.ItemDeleted += RemoveParticleSystem;
			treeView.CreateContextMenu += OnCreateContextMenu;
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}
		
		public void UpdateTree()
		{
			var indices = new List<TreeView.ItemIndex>();
			CreateTreeModel(indices, model.DefinitionTable.Definitions);
			treeView.SetValues(indices);
		}

		public void SelectItem(int id)
		{
			treeView.SelectItem(id, true);
		}
		
		private void CreateTreeModel(List<TreeView.ItemIndex> indices, ParticleCollection collection)
		{
			foreach (var def in collection)
			{
				var item = new TreeView.ItemIndex { Id = def.Id, Name = def.Name };
				indices.Add(item);
				
				CreateTreeModel(item.Children, def.Children);
			}
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
				controller.SelectDefinition = particleAction.CurrentDefinitionId;
				controller.SelectParameter = particleAction.CurrentParameter;
			}
		}

		public int CreateParticleSystem(string declaration, int parentId, bool undoable)
		{
			var action = new AddAction(controller, model, declaration, parentId, undoable);
			model.UndoHandler.ExecuteAction(action);
			return action.CurrentDefinitionId;
		}

		public void RemoveParticleSystem(int id)
		{
			var action = new RemoveAction(controller, model, id);
			model.UndoHandler.ExecuteAction(action);
		}

		public void OnNewDocument()
		{
			OnChangeDocument();

			if (model.DeclarationTable.DeclarationsList.Count > 0)
			{
				var decl = model.DeclarationTable.DeclarationsList[0];
				var id = CreateParticleSystem(decl.Name, 0, false);

				controller.SelectDefinition = id;
				//model.DefinitionTable.Definitions.GetById(id).Parameters["Texture"] = new ParticleDefinition.Parameter("Texture", new Any(new FilePath("Resources/texture.png")));
			}

			model.Modified = false;
		}

		public void OnOpenDocument()
		{
			OnChangeDocument();
			controller.SelectDefinition = 1;
		}

		private void OnChangeDocument()
		{
			model.DefinitionIdCounter = GetHighestId(model.DefinitionTable.Definitions, 0) + 1;
			//Log.Info("Start ID: " + model.DefinitionIdCounter);

			model.DocumentOpen = true;
		}

		private int GetHighestId(ParticleCollection particleCollection, int highest)
		{
			foreach (var particle in particleCollection)
			{
				if (particle.Id > highest) highest = particle.Id;
				var v = GetHighestId(particle.Children, highest);

				if (v > highest)
				{
					highest = v;
				}
			}

			return highest;
		}
		
		private void OnItemSelected(int id)
		{
			var def = model.DefinitionTable.Definitions.GetById(id);
			model.CurrentDefinitionId = id;
			ItemSelected(def);
		}
		
		private bool OnItemRenamed(int id, string newText)
		{
			var def = model.DefinitionTable.Definitions.GetById(id);
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
		
		private void OnItemMoved(int movedItemId)
		{
			var indices = treeView.GetItemIndices();
			
			int newIndex;
			int newParent;

			if (!HasChange(model.DefinitionTable.Definitions, indices)) return;
			if (!FindItemLocation(indices, movedItemId, 0, out newIndex, out newParent)) return;

			var moveAction = new MoveAction(controller, model, movedItemId, newIndex, newParent);
			model.UndoHandler.ExecuteAction(moveAction);
		}
		
		private bool FindItemLocation(List<TreeView.ItemIndex> indices, int itemId, int parentId, out int newIndex, out int newParent)
		{
			for (int i = 0; i < indices.Count; i++)
			{
				var index = indices[i];
				if (index.Id == itemId)
				{
					newIndex = i;
					newParent = parentId;
					return true;
				}

				if (FindItemLocation(index.Children, itemId, index.Id, out newIndex, out newParent))
				{
					return true;
				}
			}

			newIndex = -1;
			newParent = 0;
			return false;
		}

		private bool HasChange(ParticleCollection definitions, List<TreeView.ItemIndex> indices)
		{
			for (var i = 0; i < definitions.Count; i++)
			{
				if (definitions.Count != indices.Count || definitions[i].Id != indices[i].Id)
				{
					return true;
				}

				if (HasChange(definitions[i].Children, indices[i].Children))
				{
					return true;
				}
			}

			return false;
		}
		
		private void OnCreateContextMenu(TreeView.ContextMenu contextMenu)
		{
			var def = model.DefinitionTable.Definitions.GetById(contextMenu.ItemId);

			var declarations = model.DeclarationTable.DeclarationsList;
			if (declarations.Count > 0)
			{
				var addEntry = new TreeView.ContextMenu.Entry("Add...", null);
				addEntry.SubEntries = new List<TreeView.ContextMenu.Entry>();

				foreach (var decl in declarations)
				{
					var d = decl;
					addEntry.SubEntries.Add(new TreeView.ContextMenu.Entry(decl.Name, () => OnContextMenuAdd(d.Name, contextMenu.ItemId)));
				}

				contextMenu.Entries.Add(addEntry);
			}
			
			if (def != null)
			{
				contextMenu.Entries.Add(new TreeView.ContextMenu.Entry("Remove \"" + def.Name + "\"", () => OnContextMenuRemove(def.Id)));
			}
		}

		private void OnContextMenuAdd(string declaration, int parentId)
		{
			CreateParticleSystem(declaration, parentId, true);
		}

		private void OnContextMenuRemove(int id)
		{
			RemoveParticleSystem(id);
		}
	}
}
