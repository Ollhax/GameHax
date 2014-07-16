using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleDefinition : IEquatable<ParticleDefinition>
	{
		public class Parameter : IEquatable<Parameter>
		{
			public string Name;
			public Any Value;
			public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

			public Parameter()
			{

			}

			public Parameter(Parameter other)
			{
				Name = other.Name;
				Value = new Any(other.Value);

				Parameters.Clear();
				foreach (var p in other.Parameters)
				{
					Parameters.Add(p.Key, new Parameter(p.Value));
				}
			}

			public Parameter(XmlNode node)
			{
				Name = XmlHelper.ReadString(node, "Name");
				Value = new Any(XmlHelper.ReadString(node, "Value"), XmlHelper.ReadString(node, "Type"));

				var parametersNode = node.SelectSingleNode("Parameters");
				if (parametersNode != null)
				{
					foreach (XmlNode parameterNode in parametersNode)
					{
						var parameter = new Parameter(parameterNode);
						Parameters.Add(parameter.Name, parameter);
					}
				}
			}

			public void CopyFrom(Parameter other)
			{
				Name = other.Name;
				Value.CopyFrom(other.Value);

				if (Parameters.Count != other.Parameters.Count)
				{
					Parameters.Clear();
					foreach (var p in other.Parameters)
					{
						Parameters.Add(p.Key, new Parameter(p.Value));
					}
				}

				foreach (var p in other.Parameters)
				{
					Parameters[p.Key].CopyFrom(p.Value);
				}
			}

			public bool Equals(Parameter other)
			{
				if (Name != other.Name) return false;
				if (!Value.Equals(other.Value)) return false;

				foreach (var param in Parameters)
				{
					Parameter otherParam;
					if (!other.Parameters.TryGetValue(param.Key, out otherParam)) return false;
					if (!param.Value.Equals(otherParam)) return false;
				}

				return true;
			}

			public void Save(XmlNode node)
			{
				var document = node.OwnerDocument ?? (XmlDocument)node;

				var parameterNode = document.CreateElement("Parameter");
				node.AppendChild(parameterNode);

				XmlHelper.Write(parameterNode, "Name", Name);
				XmlHelper.Write(parameterNode, "Type", Value.GetTypeOfValue().Name);
				XmlHelper.Write(parameterNode, "Value", Value.ToString());

				var parametersNode = document.CreateElement("Parameters");
				parameterNode.AppendChild(parametersNode);

				foreach (var property in Parameters)
				{
					property.Value.Save(parametersNode);
				}
			}
		}

		public int Id;
		public string Name;
		public string Emitter;
		public string Declaration;
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
		public ParticleCollection Children = new ParticleCollection();
		public ParticleDefinition Parent;

		private Dictionary<string, RandomFloat> cachedFloats = new Dictionary<string, RandomFloat>();
		private Dictionary<string, RandomInt> cachedInts = new Dictionary<string, RandomInt>();

		public Parameter GetParameter(string name)
		{
			Parameter param;
			if (Parameters.TryGetValue(name, out param))
			{
				return param;
			}

			throw new Exception("Missing particle parameter: " + name);
		}

		public void CopyFrom(ParticleDefinition other)
		{
			Name = other.Name;
			Emitter = other.Emitter;
			Declaration = other.Declaration;
			Id = other.Id;

			if (Parameters.Count != other.Parameters.Count)
			{
				Parameters.Clear();
				foreach (var p in other.Parameters)
				{
					Parameters.Add(p.Key, new Parameter(p.Value));
				}
			}

			foreach (var p in other.Parameters)
			{
				Parameters[p.Key].CopyFrom(p.Value);
			}

			if (Children.Count != other.Children.Count)
			{
				Children.Clear();
				foreach (var s in other.Children)
				{
					Children.Add(new ParticleDefinition(s));
				}
			}

			Parent = other.Parent;
		}

		public bool Equals(ParticleDefinition other)
		{
			if (Name != other.Name) return false;
			if (Emitter != other.Emitter) return false;
			if (Declaration != other.Declaration) return false;
			if (Id != other.Id) return false;
			if (Parameters.Count != other.Parameters.Count) return false;

			foreach (var param in Parameters)
			{
				Parameter otherParam;
				if (!other.Parameters.TryGetValue(param.Key, out otherParam)) return false;
				if (!param.Value.Equals(otherParam)) return false;
			}

			return true;
		}

		public ParticleDefinition()
		{

		}

		public ParticleDefinition(ParticleDefinition other)
		{
			CopyFrom(other);
		}

		public ParticleDefinition(XmlNode node)
		{
			Name = XmlHelper.ReadString(node, "Name");
			Emitter = XmlHelper.ReadString(node, "Emitter");
			Declaration = XmlHelper.ReadString(node, "Declaration");

			var parametersNode = node.SelectSingleNode("Parameters");
			if (parametersNode != null)
			{
				foreach (XmlNode parameterNode in parametersNode)
				{
					var parameter = new Parameter(parameterNode);
					Parameters.Add(parameter.Name, parameter);
				}
			}

			var childrenNode = node.SelectSingleNode("Children");
			if (childrenNode != null)
			{
				foreach (XmlNode childNode in childrenNode)
				{
					var def = new ParticleDefinition(childNode);
					def.Parent = this;
					Children.Add(def);
				}
			}
		}

		public void Save(XmlNode node)
		{
			var document = node.OwnerDocument ?? (XmlDocument)node;

			var particleSystemNode = document.CreateElement("ParticleSystem");
			node.AppendChild(particleSystemNode);

			XmlHelper.Write(particleSystemNode, "Name", Name);
			XmlHelper.Write(particleSystemNode, "Emitter", Emitter);
			XmlHelper.Write(particleSystemNode, "Declaration", Declaration);

			var parametersNode = document.CreateElement("Parameters");
			particleSystemNode.AppendChild(parametersNode);
			foreach (var property in Parameters)
			{
				property.Value.Save(parametersNode);
			}

			var childrenNode = document.CreateElement("Children");
			particleSystemNode.AppendChild(childrenNode);

			foreach (var child in Children)
			{
				child.Save(childrenNode);
			}
		}

		public string Serialize()
		{
			using (var stringWriter = new StringWriter())
			using (var xmlTextWriter = XmlWriter.Create(stringWriter, XmlHelper.DefaultWriterSettings))
			{
				var document = new XmlDocument();

				Save(document);
				document.Save(xmlTextWriter);
				xmlTextWriter.Flush();

				return stringWriter.ToString();
			}
		}

		public void ReloadCache()
		{
			foreach (var f in cachedFloats.Values)
			{
				f.Reload();
			}

			foreach (var f in cachedInts.Values)
			{
				f.Reload();
			}
		}

		internal RandomFloat GetFloatParameter(string parameterName)
		{
			RandomFloat f;
			if (cachedFloats.TryGetValue(parameterName, out f)) return f;

			f = new RandomFloat(this, parameterName);
			cachedFloats.Add(parameterName, f);
			return f;
		}

		internal RandomInt GetIntParameter(string parameterName)
		{
			RandomInt f;
			if (cachedInts.TryGetValue(parameterName, out f)) return f;

			f = new RandomInt(this, parameterName);
			cachedInts.Add(parameterName, f);
			return f;
		}
	}

	public class ParticleDefinitionTable
	{
		public ParticleCollection Definitions = new ParticleCollection();

		public void Load(string file)
		{
			using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Load(fs);
			}
		}

		public bool Load(Stream stream)
		{
			using (var xmlReader = XmlReader.Create(stream, XmlHelper.DefaultReaderSettings))
			{
				var document = new XmlDocument();
				document.Load(xmlReader);
				return Load(document.DocumentElement);
			}
		}

		public bool Load(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "ParticleSystem")
				{
					var definition = new ParticleDefinition(child);
					Definitions.Add(definition);
				}
			}

			int id = 1;
			AssignIds(Definitions, ref id);
			return true;
		}

		private void AssignIds(ParticleCollection particleCollection, ref int id)
		{
			foreach (var child in particleCollection)
			{
				child.Id = id;
				id++;

				AssignIds(child.Children, ref id);
			}
		}

		public void Save(string file)
		{
			using (FileStream fs = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Save(fs);
			}
		}

		public void Save(Stream stream)
		{
			using (var xmlWriter = XmlWriter.Create(stream, XmlHelper.DefaultWriterSettings))
			{
				var document = new XmlDocument();
				Save(document);
				document.Save(xmlWriter);
			}
		}

		public void Save(XmlNode node)
		{
			var document = node.OwnerDocument ?? (XmlDocument)node;

			var tableNode = document.CreateElement("ParticleSystemTable");
			node.AppendChild(tableNode);

			foreach (var child in Definitions)
			{
				child.Save(tableNode);
			}
		}
	}
}
