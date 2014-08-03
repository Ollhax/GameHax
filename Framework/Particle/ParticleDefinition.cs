using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Numerics;
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

			public Parameter(string name, Any value)
			{
				Name = name;
				Value = value;
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

			//public void Save(XmlNode node)
			//{
			//    var document = node.OwnerDocument ?? (XmlDocument)node;

			//    var parameterNode = document.CreateElement("Parameter");
			//    node.AppendChild(parameterNode);

			//    XmlHelper.Write(parameterNode, "Name", Name);
			//    XmlHelper.Write(parameterNode, "Type", Value.GetTypeOfValue().Name);
			//    XmlHelper.Write(parameterNode, "Value", Value.ToString());
				
			//    var parametersNode = document.CreateElement("Parameters");
			//    parameterNode.AppendChild(parametersNode);

			//    foreach (var property in Parameters)
			//    {
			//        property.Value.Save(parametersNode);
			//    }
			//}
		}

		public int Id;
		public string Name;
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

		private void RegisterDefaultParameter(string name, object value)
		{
			if (Parameters.ContainsKey(name)) return;

			var parameter = new Parameter();
			parameter.Name = name;
			parameter.Value = new Any(0);
			parameter.Value.Set(value);
			Parameters.Add(parameter.Name, parameter);
		}

		private void CreateDefaults()
		{
			var defaultGradient = new Gradient();
			defaultGradient.Add(new GradientEntry(0, new Color(255, 255, 255, 255)));
			defaultGradient.Add(new GradientEntry(1, new Color(255, 255, 255, 0)));
			RegisterDefaultParameter("Description", "");
			RegisterDefaultParameter("Texture", new FilePath());
			RegisterDefaultParameter("TextureAnchorX", 0.5f);
			RegisterDefaultParameter("TextureAnchorY", 0.5f);
			RegisterDefaultParameter("TextureCellsX", 1);
			RegisterDefaultParameter("TextureCellsY", 1);
			RegisterDefaultParameter("TextureFrameTime", 0.5f);
			RegisterDefaultParameter("EmitterLife", 1.0f);
			RegisterDefaultParameter("EmitterLoop", 0);
			RegisterDefaultParameter("EmitterSpawnDelay", 0.0f);
			RegisterDefaultParameter("EmitterSpawnRate", 10.0f);
			RegisterDefaultParameter("EmitterCount", 0);
			RegisterDefaultParameter("EmitterDirection", 0.0f);
			RegisterDefaultParameter("EmitterRange", 360.0f);
			RegisterDefaultParameter("EmitterOffsetX", 0.0f);
			RegisterDefaultParameter("EmitterOffsetY", 0.0f);
			RegisterDefaultParameter("EmitterInitialSpeed", 50.0f);
			RegisterDefaultParameter("EmitterInitialRotation", 0.0f);
			RegisterDefaultParameter("EmitterInitialRotationSpeed", 0.0f);
			RegisterDefaultParameter("EmitterInitialScale", 1.0f);
			RegisterDefaultParameter("ParticleLife", 1.0f);
			RegisterDefaultParameter("ParticleInfinite", false);
			RegisterDefaultParameter("ParticleOrientToVelocity", false);
			RegisterDefaultParameter("ParticleRelativeToParent", false);
			RegisterDefaultParameter("ParticleColor", defaultGradient);
			RegisterDefaultParameter("ParticleGravityScale", 0.0f);
			RegisterDefaultParameter("ParticleAccelerationX", 0.0f);
			RegisterDefaultParameter("ParticleAccelerationY", 0.0f);
			RegisterDefaultParameter("ParticleAccelerationAngular", 0.0f);
			RegisterDefaultParameter("ParticleAirResistance", 0.0f);
			RegisterDefaultParameter("ParticleTurn", 0.0f);
			RegisterDefaultParameter("ParticleScale", 1.0f);
			RegisterDefaultParameter("ParticleScaleX", 1.0f);
			RegisterDefaultParameter("ParticleScaleY", 1.0f);
			RegisterDefaultParameter("ParticleTurbulenceStrength", 0.0f);
			RegisterDefaultParameter("ParticleTurbulenceScale", 1.0f);
			RegisterDefaultParameter("ParticleTurbulenceSpeed", 1.0f);
			RegisterDefaultParameter("SortMode", 0);
			RegisterDefaultParameter("BlendMode", 1);
		}

		public ParticleDefinition(XmlNode node)
		{
			Name = XmlHelper.ReadString(node, "Name");
			Declaration = XmlHelper.ReadString(node, "Declaration");

			if (Declaration != "Group")
			{
				CreateDefaults(); // Create defaults now to ensure the correct parameter order.
			}

			var parametersNode = node.SelectSingleNode("Parameters");
			if (parametersNode != null)
			{
				foreach (XmlNode parameterNode in parametersNode)
				{
					var parameter = new Parameter(parameterNode);
					Parameters.Remove(parameter.Name);
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
		public const int CurrentVersion = 1;
		
		public ParticleCollection Definitions = new ParticleCollection();
		
		public static XmlNode Open(string file)
		{
			using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var xmlReader = XmlReader.Create(fs, XmlHelper.DefaultReaderSettings))
				{
					var document = new XmlDocument();
					document.Load(xmlReader);
					return document.DocumentElement;
				}
			}
		}

		public static int GetVersion(XmlNode node)
		{
			return XmlHelper.ReadAttributeInt(node, "Version", 0);
		}
		
		public bool Load(string file)
		{
			var node = Open(file);
			return Load(node);
		}
		
		public bool Load(XmlNode node)
		{
			//Version = XmlHelper.ReadAttributeInt(node, "Version", Version);
			//int fileVersion = XmlHelper.ReadAttributeInt(node, "Version", 0);
			//if (fileVersion > Version)
			//{
			//    throw new ParticleVersionException("Cannot load particle of version " + fileVersion + ".\nCurrent version: " + Version);
			//}

			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "ParticleEffect")
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
	}
}
