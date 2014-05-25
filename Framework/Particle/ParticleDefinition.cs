using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

using MG.Framework.Collections;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleDefinition : IEquatable<ParticleDefinition>
	{
		public class Parameter : IEquatable<Parameter>
		{
			public string Name;
			public Any Value;
			public Any Random;

			public Parameter()
			{
				
			}

			public Parameter(Parameter other)
			{
				Name = other.Name;
				Value = new Any(other.Value);
				Random = new Any(other.Random);
			}

			public void CopyFrom(Parameter other)
			{
				Name = other.Name;
				Value.CopyFrom(other.Value);
				Random.CopyFrom(other.Random);
			}

			public bool Equals(Parameter other)
			{
				if (Name != other.Name) return false;
				if (!Value.Equals(other.Value)) return false;
				if (!Random.Equals(other.Random)) return false;
				return true;
			}
		}
		
		public string Name;
		public string Emitter;
		public string Declaration;
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
		
		// Extra data used by editor, stored here for convenience.
		public int InternalId;
		
		public void CopyFrom(ParticleDefinition other)
		{
			Name = other.Name;
			Emitter = other.Emitter;
			Declaration = other.Declaration;
			InternalId = other.InternalId;
			
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

		public bool Equals(ParticleDefinition other)
		{
			if (Name != other.Name) return false;
			if (Emitter != other.Emitter) return false;
			if (Declaration != other.Declaration) return false;
			if (InternalId != other.InternalId) return false;
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
		public OrderedDictionary<string, ParticleDefinition> Definitions = new OrderedDictionary<string, ParticleDefinition>();
		
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
					var definition = new ParticleDefinition(child);
					Definitions.Add(definition.Name, definition);
				}
			}
		}
	}
}
