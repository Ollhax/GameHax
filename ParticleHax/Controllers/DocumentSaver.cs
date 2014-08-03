using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.Framework.Utility;

namespace MG.ParticleHax.Controllers
{
	static class DocumentSaver
	{
		public static void Save(Model model, FilePath file)
		{
			using (FileStream fs = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Save(model, fs);
			}
		}

		public static void Save(Model model, Stream stream)
		{
			using (var xmlWriter = XmlWriter.Create(stream, XmlHelper.DefaultWriterSettings))
			{
				var document = new XmlDocument();
				Save(model, document);
				document.Save(xmlWriter);
			}
		}

		public static void Save(Model model, XmlNode node)
		{
			var document = node.OwnerDocument ?? (XmlDocument)node;

			var tableNode = document.CreateElement("ParticleEffectTable");
			tableNode.SetAttribute("Version", "1");
			node.AppendChild(tableNode);
			
			foreach (var child in model.DefinitionTable.Definitions)
			{
				SaveDefinition(model, tableNode, child);
			}
		}

		//public string Serialize()
		//{
		//    using (var stringWriter = new StringWriter())
		//    using (var xmlTextWriter = XmlWriter.Create(stringWriter, XmlHelper.DefaultWriterSettings))
		//    {
		//        var document = new XmlDocument();

		//        Save(document);
		//        document.Save(xmlTextWriter);
		//        xmlTextWriter.Flush();

		//        return stringWriter.ToString();
		//    }
		//}

		private static void SaveDefinition(Model model, XmlNode node, ParticleDefinition definition)
		{
			ParticleDeclaration declaration;
			if (!model.DeclarationTable.Declarations.TryGetValue(definition.Declaration, out declaration))
			{
				Log.Warning("Unknown particle type: " + definition.Declaration + ", not saved.");
				return;
			}

			var document = node.OwnerDocument ?? (XmlDocument)node;

			var particleEffectNode = document.CreateElement("ParticleEffect");
			node.AppendChild(particleEffectNode);

			XmlHelper.Write(particleEffectNode, "Name", definition.Name);
			XmlHelper.Write(particleEffectNode, "Declaration", definition.Declaration);
			
			if (definition.Parameters.Count > 0)
			{
				var parametersNode = document.CreateElement("Parameters");
				particleEffectNode.AppendChild(parametersNode);
				foreach (var parameter in definition.Parameters)
				{
					if (HasChange(parameter.Value, declaration.Parameters))
					{
						SaveParameter(model, parametersNode, parameter.Value, declaration.Parameters);
					}
				}
			}

			if (definition.Children.Count > 0)
			{
				var childrenNode = document.CreateElement("Children");
				particleEffectNode.AppendChild(childrenNode);

				foreach (var child in definition.Children)
				{
					SaveDefinition(model, childrenNode, child);
				}
			}
		}

		private static bool HasChange(ParticleDefinition.Parameter parameter, Dictionary<string, ParticleDeclaration.Parameter> defaults)
		{
			ParticleDeclaration.Parameter defaultChild;
			if (!defaults.TryGetValue(parameter.Name, out defaultChild))
			{
				return true;
			}

			if (!parameter.Value.Equals(defaultChild.DefaultValue))
			{
				return true;
			}

			foreach (var child in parameter.Parameters)
			{
				if (HasChange(child.Value, defaultChild.Parameters))
				{
					return true;
				}
			}

			return false;
		}
		
		private static void SaveParameter(Model model, XmlNode node, ParticleDefinition.Parameter parameter, Dictionary<string, ParticleDeclaration.Parameter> defaults)
		{
			ParticleDeclaration.Parameter defaultChild;
			if (!defaults.TryGetValue(parameter.Name, out defaultChild))
			{
				Log.Warning("Parameter not registered in declaration: " + parameter.Name);
				return;
			}

			var document = node.OwnerDocument ?? (XmlDocument)node;

			var parameterNode = document.CreateElement("Parameter");
			node.AppendChild(parameterNode);

			XmlHelper.Write(parameterNode, "Name", parameter.Name);
			XmlHelper.Write(parameterNode, "Type", parameter.Value.GetTypeOfValue().Name);
			XmlHelper.Write(parameterNode, "Value", parameter.Value.ToString());

			if (parameter.Parameters.Count > 0)
			{
				XmlElement parametersNode = null;
				
				foreach (var childParam in parameter.Parameters)
				{
					if (HasChange(childParam.Value, defaultChild.Parameters))
					{
						if (parametersNode == null) // Don't create the node until we know there's a child with changes.
						{
							parametersNode = document.CreateElement("Parameters");
							parameterNode.AppendChild(parametersNode);
						}

						SaveParameter(model, parametersNode, childParam.Value, defaultChild.Parameters);
					}
				}
			}
		}
	}
}
