using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Utility;

namespace MG.EditorCommon
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
			public Any ValueStep;
			public Any MinValue;
			public Any MaxValue;
			public uint ValueDigits;
			public string FilePathFilter;
			public float CurveMin = 0;
			public float CurveMax = 1;
			public List<KeyValuePair<string, Any>> ValueList;
			public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

			public void Load(XmlNode node)
			{
				var type = XmlHelper.ReadString(node, "Type");

				Name = XmlHelper.ReadString(node, "Name");
				PrettyName = XmlHelper.ReadString(node, "PrettyName", Name);
				Description = XmlHelper.ReadString(node, "Description", "");
				Description = Description.Replace("\\n", "\n");
				
				Category = XmlHelper.ReadString(node, "Category", "");
				DefaultValue = new Any(XmlHelper.ReadString(node, "DefaultValue"), type);
				ValueDigits = XmlHelper.ReadUInt(node, "ValueDigits", 1);
				FilePathFilter = XmlHelper.ReadString(node, "FilePathFilter", "");
				CurveMin = XmlHelper.ReadFloat(node, "CurveMin", CurveMin);
				CurveMax = XmlHelper.ReadFloat(node, "CurveMax", CurveMax);
				
				if (XmlHelper.HasElement(node, "ValueStep"))
				{
					ValueStep = new Any(XmlHelper.ReadString(node, "ValueStep"), type);
				}

				if (XmlHelper.HasElement(node, "MinValue"))
				{
					MinValue = new Any(XmlHelper.ReadString(node, "MinValue"), type);
				}

				if (XmlHelper.HasElement(node, "MaxValue"))
				{
					MaxValue = new Any(XmlHelper.ReadString(node, "MaxValue"), type);
				}
				
				var valueListNode = node["ValueList"];
				if (valueListNode != null)
				{
					ValueList = new List<KeyValuePair<string, Any>>();

					foreach (XmlNode valueNode in valueListNode)
					{
						var name = XmlHelper.ReadAttributeString(valueNode, "Name");
						var value = new Any(XmlHelper.ReadAttributeString(valueNode, "Value"), type);
						ValueList.Add(new KeyValuePair<string, Any>(name, value));
					}
				}

				var parametersNode = node.SelectSingleNode("Parameters");
				if (parametersNode != null)
				{
					foreach (XmlNode parameterNode in parametersNode)
					{
						var parameter = new Parameter();
						parameter.Load(parameterNode);
						Parameters.Add(parameter.Name, parameter);
					}
				}
			}

			private object GetDefaultValue(Type t)
			{
				if (t.IsValueType)
				{
					return Activator.CreateInstance(t);
				}

				return null;
			}
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
					parameter.Load(parameterNode);
					Parameters.Add(parameter.Name, parameter);

					if (parameter.DefaultValue.IsFilePath())
					{
						var filePath = parameter.DefaultValue.Get<FilePath>();
						if (!filePath.IsNullOrEmpty && !filePath.IsAbsolute)
						{
							parameter.DefaultValue.Set(filePath.ToAbsolute(Environment.CurrentDirectory));
						}
					}
				}
			}

			ConvertRelativePathsToAbsolute(Parameters);
		}

		private void ConvertRelativePathsToAbsolute(Dictionary<string, Parameter> parameters)
		{
			foreach (var paramPair in parameters)
			{
				var parameter = paramPair.Value;
				if (parameter.DefaultValue.IsFilePath())
				{
					var filePath = parameter.DefaultValue.Get<FilePath>();
					if (!filePath.IsNullOrEmpty && !filePath.IsAbsolute)
					{
						parameter.DefaultValue.Set(filePath.ToAbsolute(Environment.CurrentDirectory));
					}
				}

				ConvertRelativePathsToAbsolute(parameter.Parameters);
			}
		}
	}

	public class ParticleDeclarationTable
	{
		public Dictionary<string, ParticleDeclaration> Declarations = new Dictionary<string, ParticleDeclaration>();
		public List<ParticleDeclaration> DeclarationsList = new List<ParticleDeclaration>();

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
					var declaration = new ParticleDeclaration();
					declaration.Load(child);
					Declarations.Add(declaration.Name, declaration);
					DeclarationsList.Add(declaration);
				}
			}
		}
	}
}
