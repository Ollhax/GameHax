using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleHax.Actions;

namespace MG.ParticleHax.Controllers
{
	static class DocumentLoader
	{
		//1. måste spara relativa sökvägar när man kopierar file paths
		//2. kör AddMissingParametersRecursive på deserialiserad data
		//3. deserialize borde ske här (serialize if docummentsaver)
		
		public static void LoadDefinitionTable(Model model, FilePath file, XmlNode node)
		{
			model.DefinitionTable.Load(node);

			// Convert path parameters
			ToAbsolutePath(file.ParentDirectory, model.DefinitionTable.Definitions);

			// Add missing parameters
			AddMissingParametersRecursive(model, model.DefinitionTable.Definitions);
		}

		public static ParticleDefinition Deserialize(Model model, string serializedDefinition)
		{
			using (var stringReader = new StringReader(serializedDefinition))
			using (var xmlTextReader = XmlReader.Create(stringReader, XmlHelper.DefaultReaderSettings))
			{
				ParticleDefinition particleDefinition;
				
				try
				{
					var document = new XmlDocument();
					document.Load(xmlTextReader);
					particleDefinition = new ParticleDefinition(document.FirstChild);
				}
				catch (Exception)
				{
					return null;
				}
				
				AddMissingParametersRecursive(model, particleDefinition);
				return particleDefinition;
			}
		}

		public static void ToAbsolutePath(string directory, ParticleCollection collection)
		{
			foreach (var d in collection)
			{
				ToAbsolutePath(directory, d.Parameters);

				foreach (var c in d.Children)
				{
					ToAbsolutePath(directory, c.Parameters);
				}
			}
		}

		public static void ToAbsolutePath(string directory, Dictionary<string, ParticleDefinition.Parameter> parameters)
		{
			foreach (var param in parameters)
			{
				ToAbsolutePath(directory, param.Value);
			}
		}

		public static void ToAbsolutePath(string directory, ParticleDefinition.Parameter parameter)
		{
			var v = parameter.Value;
			if (v.IsFilePath()) // Paths are saved in relative format, convert to absolute format internally
			{
				v.Set((FilePath)v.Get<FilePath>().ToAbsolute(directory));
			}

			ToAbsolutePath(directory, parameter.Parameters);
		}
		
		private static void AddMissingParametersRecursive(Model model, ParticleCollection definitions)
		{
			foreach (var definition in definitions)
			{
				AddMissingParametersRecursive(model, definition);
			}
		}

		private static void AddMissingParametersRecursive(Model model, ParticleDefinition definition)
		{
			ParticleDeclaration declaration;
			if (model.DeclarationTable.Declarations.TryGetValue(definition.Declaration, out declaration))
			{
				AddAction.AddMissingParameters(declaration.Parameters, definition.Parameters, false);
			}
			else
			{
				Log.Warning("Could not find declation: " + declaration);
			}
			
			AddMissingParametersRecursive(model, definition.Children);
		}
	}
}
