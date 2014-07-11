using System.Collections.Generic;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleHax.Controllers;

namespace MG.ParticleHax.Actions
{
	class AddAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private string declarationName;
		private bool undoable;

		private ParticleDefinition addedDefinition;

		public AddAction(MainController controller, Model model, string declaration, bool undoable)
		{
			this.controller = controller;
			this.declarationName = declaration;
			this.model = model;
			this.undoable = undoable;
		}
		
		protected override bool CallExecute()
		{
			if (addedDefinition == null)
			{
				addedDefinition = CreateEntry(declarationName);
			}

			if (addedDefinition == null) return false;

			model.DefinitionTable.Definitions.Add(addedDefinition);
			
			CurrentDefinitionId = addedDefinition.Id;
			model.Modified = true;
			controller.UpdateTree = true;
			controller.SelectDefinition = addedDefinition.Id;

			return undoable;
		}

		protected override void CallUndo()
		{
			var collection = model.DefinitionTable.Definitions.GetParentCollection(addedDefinition.Id);
			
			if (collection != null)
			{
				model.Modified = true;
				collection.Remove(addedDefinition);
				controller.UpdateTree = true;
			}
		}

		private ParticleDefinition CreateEntry(string name)
		{
			ParticleDeclaration declaration;
			if (!model.DeclarationTable.Declarations.TryGetValue(name, out declaration)) return null;

			var definition = new ParticleDefinition();
			definition.Id = model.DefinitionIdCounter++;
			definition.Name = declaration.Name + definition.Id;
			definition.Declaration = name;

			AddMissingParameters(declaration.Parameters, definition.Parameters, false);
			
			return definition;
		}

		public static bool AddMissingParameters(Dictionary<string, ParticleDeclaration.Parameter> declarationParameters, Dictionary<string, ParticleDefinition.Parameter> definitionParameters, bool warnIfMissing)
		{
			bool hadMissing = false;

			foreach (var declarationParameterPair in declarationParameters)
			{
				var declarationParameter = declarationParameterPair.Value;
				ParticleDefinition.Parameter definitionParameter;

				if (!definitionParameters.TryGetValue(declarationParameter.Name, out definitionParameter))
				{
					if (warnIfMissing)
					{
						Log.Warning("Added missing parameter: " + declarationParameter.Name);
					}

					hadMissing = true;
					definitionParameter = new ParticleDefinition.Parameter();
					definitionParameter.Name = declarationParameter.Name;
					definitionParameter.Value = new Any(declarationParameter.DefaultValue);

					definitionParameters.Add(definitionParameter.Name, definitionParameter);
				}

				hadMissing |= AddMissingParameters(declarationParameter.Parameters, definitionParameter.Parameters, warnIfMissing);
			}

			return hadMissing;
		}
	}
}
