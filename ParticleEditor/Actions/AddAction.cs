using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Actions
{
	class AddAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private string declarationName;
		private int definitionId;
		private bool undoable;

		public AddAction(MainController controller, Model model, string declaration, bool undoable)
		{
			this.controller = controller;
			this.declarationName = declaration;
			this.model = model;
			this.undoable = undoable;
		}
		
		protected override bool CallExecute()
		{
			var entry = CreateEntry(declarationName, definitionId);
			if (entry == null) return false;

			definitionId = entry.Id;
			CurrentDefinitionId = definitionId;
			model.Modified = true;
			controller.UpdateTree = true;

			return undoable;
		}

		protected override void CallUndo()
		{
			var collection = model.DefinitionTable.Definitions.GetParentCollection(definitionId);
			var def = model.DefinitionTable.Definitions.GetById(definitionId);

			if (def != null && collection != null)
			{
				model.Modified = true;
				collection.Remove(def);
				controller.UpdateTree = true;
			}
		}

		private ParticleDefinition CreateEntry(string name, int overrideId)
		{
			ParticleDeclaration declaration;
			if (!model.DeclarationTable.Declarations.TryGetValue(name, out declaration)) return null;

			var definition = new ParticleDefinition();
			definition.Id = overrideId > 0 ? overrideId : model.DefinitionIdCounter++;
			definition.Name = declaration.Name + definition.Id;
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

			model.DefinitionTable.Definitions.Add(definition);
			controller.UpdateTree = true;
			return definition;
		}
	}
}
