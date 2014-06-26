using MG.Framework.Particle;
using MG.ParticleHax.Controllers;

namespace MG.ParticleHax.Actions
{
	class RemoveAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private int definitionId;

		private int parentId;
		private int parentIndex;
		private ParticleDefinition oldDef;
		
		public RemoveAction(MainController controller, Model model, int definitionId)
		{
			this.controller = controller;
			this.model = model;
			this.definitionId = definitionId;
			CurrentDefinitionId = definitionId;
		}
		
		protected override bool CallExecute()
		{
			var collection = model.DefinitionTable.Definitions.GetParentCollection(definitionId);
			var def = model.DefinitionTable.Definitions.GetById(definitionId);

			if (def != null && collection != null)
			{
				parentId = def.Parent != null ? def.Parent.Id : 0;
				parentIndex = collection.IndexOf(def);

				oldDef = def;
				collection.Remove(def);
				controller.UpdateTree = true;
				model.Modified = true;
				return true;
			}
			return false;
		}

		protected override void CallUndo()
		{
			var collection = model.DefinitionTable.Definitions;
			var parentDef = model.DefinitionTable.Definitions.GetById(parentId);
			if (parentDef != null)
			{
				collection = parentDef.Children;
			}

			collection.Insert(parentIndex, oldDef);
			controller.UpdateTree = true;
			model.Modified = true;
		}
	}
}
