using System;

using MG.Framework.Particle;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Actions
{
	class MoveAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private int definitionId;
		
		private int newParent;
		private int newIndex;
		private int oldParent;
		private int oldIndex;
		
		public string Error;

		public MoveAction(MainController controller, Model model, int definitionId, int newIndex, int newParent)
		{
			this.controller = controller;
			this.model = model;
			this.definitionId = definitionId;
			this.newIndex = newIndex;
			this.newParent = newParent;
			
			CurrentDefinitionId = definitionId;

			var def = model.DefinitionTable.Definitions.GetById(definitionId);
			if (def != null)
			{
				oldIndex = model.DefinitionTable.Definitions.IndexOfRecursive(def);
				oldParent = def.Parent != null ? def.Parent.InternalId : 0;
			}
			else
			{
				throw new ArgumentException("Cannot find definition "+ definitionId);
			}
		}

		private ParticleCollection GetParentCollection(int parentId)
		{
			var collection = model.DefinitionTable.Definitions;
			var def = model.DefinitionTable.Definitions.GetById(parentId);
			if (def != null)
			{
				collection = def.Children;
			}
			return collection;
		}

		protected override bool CallExecute()
		{
			var oldCollection = GetParentCollection(oldParent);
			var newCollection = GetParentCollection(newParent);
			
			var def = model.DefinitionTable.Definitions.GetById(definitionId);
			if (def != null)
			{
				oldCollection.RemoveAt(oldIndex);
				newCollection.Insert(newIndex, def);
				def.Parent = model.DefinitionTable.Definitions.GetById(newParent);
				controller.SelectDefinition = definitionId;
				controller.UpdateTree = true;
				return true;
			}
			
			return false;
		}

		protected override void CallUndo()
		{
			var oldCollection = GetParentCollection(oldParent);
			var newCollection = GetParentCollection(newParent);

			var def = model.DefinitionTable.Definitions.GetById(definitionId);
			if (def != null)
			{
				newCollection.RemoveAt(newIndex);
				oldCollection.Insert(oldIndex, def);
				def.Parent = model.DefinitionTable.Definitions.GetById(oldParent);
				controller.UpdateTree = true;
			}
		}
	}
}
