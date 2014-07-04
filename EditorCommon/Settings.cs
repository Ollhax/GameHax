using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using MG.Framework.Utility;

namespace MG.EditorCommon
{
	public static class Settings
	{
		private static Dictionary<string, Any> settings = new Dictionary<string, Any>();

		public static FilePath File = "settings.xml";

		public static FilePath Path
		{
			get { return Framework.Framework.SaveDataFolder.Combine(File); }
		}
		
		public static T Get<T>(string name)
		{
			Any value;
			if (settings.TryGetValue(name, out value))
			{
				return value.Get<T>();
			}

			return default(T);
		}

		public static void Set<T>(string name, T value)
		{
			settings[name] = new Any(value);
		}
		
		public static void Load()
		{
			try
			{
				using (FileStream fs = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var xmlReader = XmlReader.Create(fs, XmlHelper.DefaultReaderSettings))
					{
						var document = new XmlDocument();
						document.Load(xmlReader);
						Load(document.DocumentElement);
					}
				}
			}
			catch (Exception e)
			{
				Log.Info("Could not load settings: " + e.Message);
			}
		}

		private static void Load(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				var name = XmlHelper.ReadAttributeString(child, "Name");				
				var value = XmlHelper.ReadAttributeString(child, "Value");
				var type = XmlHelper.ReadAttributeString(child, "Type");
				settings[name] = new Any(value, type);
			}
		}

		public static void Save()
		{
			try
			{
				using (FileStream fs = System.IO.File.Open(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
				{
					using (var xmlWriter = XmlWriter.Create(fs, XmlHelper.DefaultWriterSettings))
					{
						var document = new XmlDocument();
						Save(document);
						document.Save(xmlWriter);
					}
				}
			}
			catch (Exception e)
			{
				Log.Info("Could not save settings: " + e.Message);
				throw;
			}
		}

		private static void Save(XmlNode node)
		{
			var settingsNode = XmlHelper.CreateChildNode(node, "Settings");

			foreach (var setting in settings)
			{
				var child = XmlHelper.CreateChildNode(settingsNode, "Setting");
				XmlHelper.WriteAttribute(child, "Name", setting.Key);
				XmlHelper.WriteAttribute(child, "Value", setting.Value.ToString());
				XmlHelper.WriteAttribute(child, "Type", setting.Value.GetTypeOfValue().Name);
			}
		}
	}
}
