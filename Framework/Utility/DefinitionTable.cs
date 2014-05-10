using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace MG.Framework.Utility
{
	/// <summary>
	/// Generic definition base class.
	/// </summary>
	[Serializable]
	public abstract class Definition
	{
		public string Name;
		
		/// <summary>
		/// Load data from Xml.
		/// </summary>
		/// <param name="node">Xml data.</param>
		public virtual void Load(XmlNode node)
		{
			Name = XmlHelper.ReadAttributeString(node, "Name", Name);
		}

		/// <summary>
		/// Clone this definition.
		/// </summary>
		/// <returns>A clone of this definition.</returns>
		public Definition Clone()
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, this);
				ms.Position = 0;

				return (Definition)formatter.Deserialize(ms);
			}
		}
	}

	/// <summary>
	/// Generic definition table base class.
	/// </summary>
	/// <typeparam name="T">The definition type.</typeparam>
	public class DefinitionTable<T> 
		where T : Definition, new()
	{
		private Dictionary<string, T> entriesByName = new Dictionary<string, T>();
		private List<T> entriesByIndex = new List<T>();

		/// <summary>
		/// Get the specified definition.
		/// </summary>
		/// <param name="name">Name of the definition.</param>
		/// <returns>The definition.</returns>
		public T GetDefinition(string name)
		{
			T def;
			if (entriesByName.TryGetValue(name, out def))
			{
				return def;
			}

			Log.Fatal("Missing definition: " + name);
			return null;
		}

		/// <summary>
		/// Get the specified definition.
		/// </summary>
		/// <param name="index">Index of the definition.</param>
		/// <returns>The definition.</returns>
		public T GetDefinition(int index)
		{
			if (index < 0 || index >= entriesByIndex.Count)
				Log.Fatal("Invalid definition index: " + index);
			
			return entriesByIndex[index];
		}

		/// <summary>
		/// Try getting the specified definition.
		/// </summary>
		/// <param name="name">Name of the definition.</param>
		/// <returns>The definition, or null if the definition is not found.</returns>
		public T TryGetDefinition(string name)
		{
			T def;
			if (entriesByName.TryGetValue(name, out def))
			{
				return def;
			}
			return null;
		}
		
		/// <summary>
		/// Test if a named definition exists.
		/// </summary>
		/// <param name="name">Name of definition.</param>
		/// <returns>True if the named definition exists.</returns>
		public bool HasDefinition(string name)
		{
			return entriesByName.ContainsKey(name);
		}

		/// <summary>
		/// Test if a definition exists at the specified index.
		/// </summary>
		/// <param name="index">Index of definition.</param>
		/// <returns>True if there is a definition by the specified index.</returns>
		public bool HasDefinition(int index)
		{
			return index >= 0 && index < entriesByIndex.Count;
		}

		/// <summary>
		/// Get the index of the named definition.
		/// </summary>
		/// <param name="name">Name of definition.</param>
		/// <returns>The index of the named definition, or -1 if it cannot be found.</returns>
		public int GetDefinitionIndex(string name)
		{
			for (int i = 0; i < entriesByIndex.Count; i++)
			{
				if (entriesByIndex[i].Name == name)
				{
					return i;
				}
			}

			Log.Info("Could not find definition: " + name);
			return -1;
		}

		/// <summary>
		/// Get the full list of definitions in this table.
		/// </summary>
		/// <returns>All the definitions.</returns>
		public List<T> GetDefinitionList()
		{
			return entriesByIndex;
		}

		/// <summary>
		/// Load definitions from file.
		/// </summary>
		/// <param name="file"></param>
		public void Load(string file)
		{
			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var xmlReader = XmlReader.Create(stream, XmlHelper.DefaultReaderSettings))
			{
				var document = new XmlDocument();
				document.Load(xmlReader);

				if (document.DocumentElement != null)
				{
					foreach (XmlNode child in document.DocumentElement)
					{
						T def = null;
						var template = XmlHelper.ReadAttributeString(child, "Template");
						
						if (!string.IsNullOrEmpty(template))
						{
							if (!entriesByName.TryGetValue(template, out def))
							{
								Log.Error("Missing template: " + template);
							}
							else
							{
								def = (T)def.Clone();
							}
						}

						if (def == null)
						{
							def = new T();
						}
						
						def.Load(child);

						if (entriesByName.ContainsKey(def.Name))
						{
							Log.Error("Duplicate entry: " + def.Name);
						}
						
						entriesByName[def.Name] = def;
						entriesByIndex.Add(def);
					}
				}
			}
		}
	}
}
