using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleDefinition
	{
		public class Parameter
		{
			public string Name;
			public Any Value;
			public Any Random;
		}
		
		public string Name;
		public string Emitter;
		public string Declaration;
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

		public void Load(XmlNode node)
		{
			Name = XmlHelper.ReadString(node, "Name");
			Emitter = XmlHelper.ReadString(node, "Emitter");
			Declaration = XmlHelper.ReadString(node, "Declaration");

			var parametersNode = node.SelectSingleNode("Parameters");
			if (parametersNode != null)
			{
				foreach (XmlNode parameterNode in parametersNode)
				{
					var parameter = new Parameter();

					parameter.Name = XmlHelper.ReadString(parameterNode, "Name");
					parameter.Value = new Any(
						XmlHelper.ReadString(parameterNode, "Value"), XmlHelper.ReadString(parameterNode, "Type"));

					if (XmlHelper.HasElement(parameterNode, "Random"))
					{
						parameter.Random = new Any(XmlHelper.ReadString(parameterNode, "Random"), XmlHelper.ReadString(parameterNode, "Type"));
					}

					Parameters.Add(parameter.Name, parameter);
				}
			}
		}
	}

	public class ParticleDefinitionTable
	{
		public Dictionary<string, ParticleDefinition> Definitions = new Dictionary<string, ParticleDefinition>();
		
		public void Load(string file)
		{
			try
			{
				using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					Load(fs);
				}
			}
			catch (Exception e)
			{
				Log.Error("- Error: " + e.Message);
			}
		}

		public void Load(Stream stream)
		{
			using (var xmlReader = XmlReader.Create(stream, XmlHelper.DefaultReaderSettings))
			{
				var document = new XmlDocument();
				document.Load(xmlReader);
				Load(document.DocumentElement);
			}
		}

		public void Load(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "ParticleSystem")
				{
					var definition = new ParticleDefinition();
					definition.Load(child);
					Definitions.Add(definition.Name, definition);
				}
			}
		}
	}
}
