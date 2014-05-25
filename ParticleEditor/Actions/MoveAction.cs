using System;

using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Actions
{
	class MoveAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private int definitionId;
		
		private int newIndex;
		private int oldIndex;
		
		public string Error;

		public MoveAction(MainController controller, Model model, int definitionId, int newIndex)
		{
			this.controller = controller;
			this.model = model;
			this.definitionId = definitionId;
			this.newIndex = newIndex;

			CurrentDefinitionId = definitionId;

			var def = model.GetDefinitionById(definitionId);
			if (def != null)
			{
				oldIndex = model.DefinitionTable.Definitions.IndexOfKey(def.Name);
			}
			else
			{
				throw new ArgumentException("Cannot find definition "+ definitionId);
			}
		}

		protected override bool CallExecute()
		{
			var def = model.GetDefinitionById(definitionId);
			if (def != null)
			{
				model.DefinitionTable.Definitions.RemoveAt(oldIndex);
				model.DefinitionTable.Definitions.Insert(newIndex, def.Name, def);
				controller.UpdateTree = true;
				return true;
			}
			
			return false;
		}

		protected override void CallUndo()
		{
			var def = model.GetDefinitionById(definitionId);
			if (def != null)
			{
				model.DefinitionTable.Definitions.RemoveAt(newIndex);
				model.DefinitionTable.Definitions.Insert(oldIndex, def.Name, def);
				controller.UpdateTree = true;
			}
		}
	}
}
