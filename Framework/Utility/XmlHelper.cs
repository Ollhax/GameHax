using System;
using System.Xml;

using MG.Framework.Numerics;
using MG.Framework.Converters;

namespace MG.Framework.Utility
{
	public class XmlHelperException : Exception
	{
		public XmlHelperException(string message) : base(message) { }
	}

	public class XmlHelperMissingRequiredElement : XmlHelperException
	{
		public XmlHelperMissingRequiredElement(string element, XmlNode node) : base("Missing required element \"" + element + "\" in node \"" + node.Name + "\".") { }
	}

	public class XmlHelperMissingRequiredAttribute : XmlHelperException
	{
		public XmlHelperMissingRequiredAttribute(string attribute, XmlNode node) : base("Missing required attribute \"" + attribute + "\" in element \"" + node.Name + "\".") { }
	}

	public class XmlHelperInvalidElementType : XmlHelperException
	{
		public XmlHelperInvalidElementType(string element, XmlNode node, string exceptionMessage) : base("Invalid element \"" + element + "\" in node \"" + node.Name + "\": " + exceptionMessage) { }
	}

	/// <summary>
	/// Helper for parsing XML documents.
	/// </summary>
	public static class XmlHelper
	{
		static XmlWriterSettings writerSettings = new XmlWriterSettings { IndentChars = "\t", Indent = true };
		static XmlReaderSettings readerSettings = new XmlReaderSettings { IgnoreComments = true };

		public static XmlWriterSettings DefaultWriterSettings { get { return writerSettings; } }
		public static XmlReaderSettings DefaultReaderSettings { get { return readerSettings; } }

		public static XmlNode CreateChildNode(XmlNode node, string nodeName)
		{
			XmlDocument document = node.OwnerDocument ?? (XmlDocument)node;
			var newNode = document.CreateElement(nodeName);
			node.AppendChild(newNode);
			return newNode;
		}

		public static bool HasAttribute(XmlNode node, string attribute)
		{
			return node.Attributes[attribute] != null;
		}

		public static bool HasElement(XmlNode node, string element)
		{
			var child = node[element];
			if (child == null)
			{
				return false;
			}
			return true;
		}

		public static string ReadString(XmlNode node, string element, string defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				return child.InnerText;
			}

			if (defaultValue != null)
			{
				return defaultValue;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static int ReadInt(XmlNode node, string element, int? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				try
				{
					return Convert.ToInt32(child.InnerText);
				}
				catch (FormatException e)
				{
					throw new XmlHelperInvalidElementType(element, node, e.Message);
				}
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static uint ReadUInt(XmlNode node, string element, uint? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				try
				{
					return Convert.ToUInt32(child.InnerText);
				}
				catch (FormatException e)
				{
					throw new XmlHelperInvalidElementType(element, node, e.Message);
				}
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static long ReadLong(XmlNode node, string element, long? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				try
				{
					return Convert.ToInt64(child.InnerText);
				}
				catch (FormatException e)
				{
					throw new XmlHelperInvalidElementType(element, node, e.Message);
				}
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static float ReadFloat(XmlNode node, string element, float? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				float ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to Float.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static bool ReadBool(XmlNode node, string element, bool? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				try
				{
					return Convert.ToBoolean(child.InnerText);
				}
				catch (FormatException e)
				{
					throw new XmlHelperInvalidElementType(element, node, e.Message);
				}
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static Vector2 ReadVector2(XmlNode node, string element, Vector2? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				Vector2 ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to Vector2.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static Vector2I ReadVector2I(XmlNode node, string element, Vector2I? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				Vector2I ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to Vector2I.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static RelativePosition2 ReadRelativePosition2(XmlNode node, string element, RelativePosition2? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				RelativePosition2 ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to RelativePosition2.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static Color ReadColor(XmlNode node, string element, Color? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				Color ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to Color.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		public static Rectangle ReadRectangle(XmlNode node, string element, Rectangle? defaultValue = null)
		{
			var child = node[element];
			if (child != null)
			{
				Rectangle ret;
				if (!TypeConvert.FromString(child.InnerText, out ret))
				{
					throw new XmlHelperInvalidElementType(element, node, "Cannot convert value to Rectangle.");
				}
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredElement(element, node);
		}

		//public static string ReadAttributeString(XmlNode node, string element, string attribute, string defaultValue = "")
		//{
		//    var child = node[element];
		//    if (child != null)
		//    {
		//        if (child.HasAttribute(attribute))
		//        {
		//            return child.GetAttribute(attribute);
		//        }

		//        if (defaultValue != null)
		//        {
		//            return defaultValue;
		//        }

		//        throw new XmlHelperMissingRequiredAttribute(attribute, element, node);
		//    }

		//    if (defaultValue != null)
		//    {
		//        return defaultValue;
		//    }

		//    throw new XmlHelperMissingRequiredElement(element, node);
		//}

		public static string ReadAttributeString(XmlNode element, string attribute, string defaultValue = "")
		{
			var attr = element.Attributes[attribute];
			if (attr != null)
			{
				return attr.Value;
			}

			if (defaultValue != null)
			{
				return defaultValue;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static uint ReadAttributeUInt(XmlNode element, string attribute, uint? defaultValue = null)
		{
			uint ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static int ReadAttributeInt(XmlNode element, string attribute, int? defaultValue = null)
		{
			int ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static long ReadAttributeLong(XmlNode element, string attribute, long? defaultValue = null)
		{
			long ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static float ReadAttributeFloat(XmlNode element, string attribute, float? defaultValue = null)
		{
			float ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static bool ReadAttributeBool(XmlNode element, string attribute, bool? defaultValue = null)
		{
			bool ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static Vector2 ReadAttributeVector2(XmlNode element, string attribute, Vector2? defaultValue = null)
		{
			Vector2 ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static Vector2I ReadAttributeVector2I(XmlNode element, string attribute, Vector2I? defaultValue = null)
		{
			Vector2I ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static RelativePosition2 ReadAttributeRelativePosition2(XmlNode element, string attribute, RelativePosition2? defaultValue = null)
		{
			RelativePosition2 ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static Color ReadAttributeColor(XmlNode element, string attribute, Color? defaultValue = null)
		{
			Color ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static Rectangle ReadAttributeRectangle(XmlNode element, string attribute, Rectangle? defaultValue = null)
		{
			Rectangle ret;
			var attr = element.Attributes[attribute];
			if (attr != null && TypeConvert.FromString(attr.Value, out ret))
			{
				return ret;
			}

			if (defaultValue.HasValue)
			{
				return defaultValue.Value;
			}

			throw new XmlHelperMissingRequiredAttribute(attribute, element);
		}

		public static void Write(XmlNode parent, string element, string value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = value;
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, int value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, uint value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, long value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, float value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, bool value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, Vector2 value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, Vector2I value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, RelativePosition2 value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, Color value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void Write(XmlNode parent, string element, Rectangle value)
		{
			var child = parent.OwnerDocument.CreateElement(element);
			child.InnerText = TypeConvert.ToString(value);
			parent.AppendChild(child);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, string value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = value;
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, int value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, uint value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, long value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, float value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, bool value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, Vector2 value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, Vector2I value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, RelativePosition2 value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, Color value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}

		public static void WriteAttribute(XmlNode parent, string attribute, Rectangle value)
		{
			var attr = parent.OwnerDocument.CreateAttribute(attribute);
			attr.Value = TypeConvert.ToString(value);
			parent.Attributes.Append(attr);
		}
	}
}
