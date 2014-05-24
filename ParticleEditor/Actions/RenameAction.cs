namespace MG.ParticleEditor.Actions
{
	class RenameAction : UndoableParticleAction
	{
		private Model model;
		private int definitionId;
		private string oldName;
		private string newName;

		public RenameAction(Model model, int definitionId, string newName)
		{
			this.newName = newName;
			this.model = model;
			this.definitionId = definitionId;

			var def = model.GetDefinitionById(definitionId);
			if (def != null)
			{
				oldName = def.Name;
			}
		}
		
		protected override bool CallExecute()
		{
			if (model.Definition.Definitions.ContainsKey(newName)) return false;
			var def = model.GetDefinitionById(definitionId);

			if (def != null)
			{
				def.Name = newName;
				model.Definition.Definitions.Remove(oldName);
				model.Definition.Definitions.Add(newName, def);
				model.UpdateTree = true;
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
				model.UpdateTree = true;
			}
		}
	}
}
