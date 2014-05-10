using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Utility;

namespace MG.ParticleEditor
{
	public class ParticleDeclaration
	{
		public class Parameter
		{
			public string Name;
			public string PrettyName;
			public string Description;
			public string Category;
			public Any DefaultValue;
			public Any DefaultValueRandom;
			public Any ValueStep;
			public Any MinValue;
			public Any MaxValue;
			public uint ValueDigits;
			public string FilePathFilter; 
			public List<KeyValuePair<string, Any>> ValueList;
		}

		public string Name;
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

		public void Load(XmlNode node)
		{
			Name = XmlHelper.ReadString(node, "Name");
			
			var parametersNode = node.SelectSingleNode("Parameters");
			if (parametersNode != null)
			{
				foreach (XmlNode parameterNode in parametersNode)
				{
					var parameter = new Parameter();
					var type = XmlHelper.ReadString(parameterNode, "Type");

					parameter.Name = XmlHelper.ReadString(parameterNode, "Name");
					parameter.PrettyName = XmlHelper.ReadString(parameterNode, "PrettyName", parameter.Name);
					parameter.Description = XmlHelper.ReadString(parameterNode, "Description", "");
					parameter.Category = XmlHelper.ReadString(parameterNode, "Category", "");
					parameter.DefaultValue = new Any(XmlHelper.ReadString(parameterNode, "DefaultValue"), type);
					parameter.ValueDigits = XmlHelper.ReadUInt(parameterNode, "ValueDigits", 1);
					parameter.FilePathFilter = XmlHelper.ReadString(parameterNode, "FilePathFilter", "");

					if (XmlHelper.HasElement(parameterNode, "DefaultValueRandom"))
					{
						parameter.DefaultValueRandom = new Any(XmlHelper.ReadString(parameterNode, "DefaultValueRandom"), type);
					}

					if (XmlHelper.HasElement(parameterNode, "ValueStep"))
					{
						parameter.ValueStep = new Any(XmlHelper.ReadString(parameterNode, "ValueStep"), type);
					}
					
					if (XmlHelper.HasElement(parameterNode, "MinValue"))
					{
						parameter.MinValue = new Any(XmlHelper.ReadString(parameterNode, "MinValue"), type);
					}

					if (XmlHelper.HasElement(parameterNode, "MaxValue"))
					{
						parameter.MaxValue = new Any(XmlHelper.ReadString(parameterNode, "MaxValue"), type);
					}

					var valueListNode = parameterNode["ValueList"];
					if (valueListNode != null)
					{
						parameter.ValueList = new List<KeyValuePair<string, Any>>();
						
						foreach (XmlNode valueNode in valueListNode)
						{
							var name = XmlHelper.ReadAttributeString(valueNode, "Name");
							var value = new Any(XmlHelper.ReadAttributeString(valueNode, "Value"), type);
							parameter.ValueList.Add(new KeyValuePair<string, Any>(name, value));
						}
					}

					Parameters.Add(parameter.Name, parameter);
				}
			}
		}
	}

	public class ParticleDeclarationTable
	{
		public Dictionary<string, ParticleDeclaration> Declarations = new Dictionary<string, ParticleDeclaration>();

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
				if (child.Name == "ParticleDeclaration")
				{
					var definition = new ParticleDeclaration();
					definition.Load(child);
					Declarations.Add(definition.Name, definition);
				}
			}
		}
	}
}
