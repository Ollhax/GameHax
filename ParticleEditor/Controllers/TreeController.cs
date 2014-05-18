using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Assets;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class TreeController
	{
		private Model model;
		private TreeView treeView;

		public event Action<ParticleDefinition> ItemSelected = delegate { };
		
		public TreeController(Model model, TreeView treeView)
		{
			this.model = model;
			this.treeView = treeView;
			
			treeView.ItemSelected += OnItemSelected;
			treeView.CreateContextMenu += OnCreateContextMenu;
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}
		
		public void CreateEntry(string declarationName)
		{
			var particleDefinition = CreateParticle(declarationName);
			model.Definition.Definitions.Add(particleDefinition.Name, particleDefinition);
			
			var items = new List<KeyValuePair<int, string>>();
			foreach (var def in model.Definition.Definitions)
			{
				items.Add(new KeyValuePair<int, string>(def.Value.InternalId, def.Key));
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
				treeView.SelectItem(particleAction.CurrentDefinitionId);
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

		private void OnCreateContextMenu(TreeView.ContextMenu contextMenu)
		{
			var def = model.GetDefinitionById(contextMenu.ItemId);
			
			contextMenu.Entries.Add(new KeyValuePair<string, Action>("Add", OnContextMenuAdd));
			if (def != null)
			{
				contextMenu.Entries.Add(new KeyValuePair<string, Action>("Remove", () => OnContextMenuRemove(def.InternalId)));
			}
		}

		private void OnContextMenuAdd()
		{
			Log.P("Add");
		}

		private void OnContextMenuRemove(int id)
		{
			Log.P("Remove " + id);
		}
	}
}
