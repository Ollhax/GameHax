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
		}
		
		protected override bool CallExecute()
		{
			if (oldName == newName)
			{
				return false;
			}

			if (model.Definition.Definitions.ContainsKey(newName))
			{
				Error = "<b>Error: Duplicate name \"" + newName + "\"</b>\n\nParticle effect names must be unique.";
				return false;
			}
			var def = model.GetDefinitionById(definitionId);

			if (def != null)
			{
				def.Name = newName;
				model.Definition.Definitions.Remove(oldName);
				model.Definition.Definitions.Add(newName, def);
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
				model.Definition.Definitions.Remove(newName);
				model.Definition.Definitions.Add(oldName, def);
				controller.UpdateTree = true;
			}
		}
	}
}
