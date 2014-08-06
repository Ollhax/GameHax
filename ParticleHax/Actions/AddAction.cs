using System;
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
		private int parentId;
		private bool undoable;

		private ParticleDefinition addedDefinition;

		public AddAction(MainController controller, Model model, string declaration, int parentId, bool undoable)
		{
			this.controller = controller;
			this.declarationName = declaration;
			this.parentId = parentId;
			this.model = model;
			this.undoable = undoable;
		}

		public AddAction(MainController controller, Model model, ParticleDefinition definition, int parentId, bool undoable)
		{
			this.controller = controller;
			this.parentId = parentId;
			this.model = model;
			this.undoable = undoable;
			this.addedDefinition = definition;

			AssignNameAndIdRecursive(model, addedDefinition);
		}
		
		protected override bool CallExecute()
		{
			if (addedDefinition == null)
			{
				addedDefinition = CreateEntry(declarationName);
			}

			if (addedDefinition == null) return false;

			ParticleCollection collection = model.DefinitionTable.Definitions;
			var parent = model.DefinitionTable.Definitions.GetById(parentId);
			if (parent != null)
			{
				collection = parent.Children;
				addedDefinition.Parent = parent;
			}

			collection.Add(addedDefinition);
			
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
			definition.Declaration = name;
			definition.Id = GenerateId(model);
			definition.Name = GenerateName(model, definition);

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

		private static void AssignNameAndIdRecursive(Model model, ParticleDefinition definition)
		{
			definition.Id = GenerateId(model);
			definition.Name = GenerateName(model, definition);

			foreach (var child in definition.Children)
			{
				AssignNameAndIdRecursive(model, child);
			}
		}

		private static int GenerateId(Model model)
		{
			return model.DefinitionIdCounter++;
		}

		private static string GenerateName(Model model, ParticleDefinition definition)
		{
			if (!string.IsNullOrEmpty(definition.Name))
			{
				int value;
				string text;
				if (GetEndDigit(definition.Name, out text, out value))
				{
					return GetUniqueName(model, definition, text, value + 1);
				}

				return GetUniqueName(model, definition, definition.Name, 1);
			}

			return GetUniqueName(model, definition, definition.Declaration, definition.Id);
		}

		private static bool GetEndDigit(string name, out string textPart, out int valuePart)
		{
			textPart = "";
			valuePart = 0;

			if (string.IsNullOrEmpty(name)) return false;

			int startDigit = name.Length - 1;
			while (startDigit >= 0)
			{
				if (!char.IsDigit(name[startDigit]))
				{
					break;
				}

				startDigit--;
			}

			if (name.Length - startDigit > 1)
			{
				try
				{
					valuePart = Convert.ToInt32(name.Substring(startDigit + 1));
					textPart = name.Substring(0, startDigit + 1);
				}
				catch (Exception)
				{
					return false;
				}

				return true;
			}

			return false;
		}

		private static string GetUniqueName(Model model, ParticleDefinition definition, string baseName, int baseCount)
		{
			while (true)
			{
				var name = baseName + baseCount;

				if (IsUniqueName(model, definition, name))
				{
					return name;
				}

				baseCount++;
			}
		}

		private static bool IsUniqueName(Model model, ParticleDefinition definition, string name)
		{
			if (!IsUniqueName(definition, name)) return false;
			return model.DefinitionTable.Definitions.GetByName(name) == null;
		}
		
		private static bool IsUniqueName(ParticleDefinition definition, string name)
		{
			if (definition.Name == name) return false;
			if (definition.Parent == null) return true;
			return IsUniqueName(definition.Parent, name);
		}
	}
}
