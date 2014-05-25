using System;

using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Actions
{
	class RenameAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private int definitionId;
		private string oldName;
		private string newName;

		public string Error;

		public RenameAction(MainController controller, Model model, int definitionId, string newName)
		{
			this.controller = controller;
			this.newName = newName;
			this.model = model;
			this.definitionId = definitionId;

			CurrentDefinitionId = definitionId;

			var def = model.GetDefinitionById(definitionId);
			if (def != null)
			{
				oldName = def.Name;
			}
			else
			{
				throw new ArgumentException("Cannot find definition " + definitionId);
			}
		}
		
		protected override bool CallExecute()
		{
			if (oldName == newName)
			{
				return false;
			}

			if (model.DefinitionTable.Definitions.ContainsKey(newName))
			{
				Error = "<b>Error: Duplicate name \"" + newName + "\"</b>\n\nParticle effect names must be unique.";
				return false;
			}
			var def = model.GetDefinitionById(definitionId);

			if (def != null)
			{
				def.Name = newName;
				model.DefinitionTable.Definitions.Remove(oldName);
				model.DefinitionTable.Definitions.Add(newName, def);
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
				def.Name = oldName;
				model.DefinitionTable.Definitions.Remove(newName);
				model.DefinitionTable.Definitions.Add(oldName, def);
				controller.UpdateTree = true;
			}
		}
	}
}
