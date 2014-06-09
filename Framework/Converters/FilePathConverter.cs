using System;
using MG.Framework.Numerics;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;

using MG.Framework.Utility;

namespace MG.Framework.Converters
{
	public class FilePathConverter : TypeConverter
	{
		private PropertyDescriptorCollection propertyDescriptor;

		public FilePathConverter()
		{
		    var type = typeof(FilePath);
			propertyDescriptor =
				new PropertyDescriptorCollection(
					new PropertyDescriptor[] { new PropertyPropertyDescriptor(type.GetProperty("String")) });
		}
		
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null)
				throw new ArgumentNullException("propertyValues");

			object boxed = Activator.CreateInstance(context.PropertyDescriptor.PropertyType);
			foreach (DictionaryEntry entry in propertyValues)
			{
				PropertyInfo pi = context.PropertyDescriptor.PropertyType.GetProperty(entry.Key.ToString());
				if ((pi != null) && (pi.CanWrite))
				{
					pi.SetValue(boxed, Convert.ChangeType(entry.Value, pi.PropertyType), null);
				}
			}
			return boxed;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return propertyDescriptor;
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return new FilePath((string)value);

				//var separator = new string[] { culture.TextInfo.ListSeparator };

				//try
				//{
				//    var v = ((string)value).Split(separator, StringSplitOptions.None);
				//    if (v.Length == 4)
				//    {
				//        var r = new RectangleF();
				//        TypeConvert.FromString(v[0].Trim(), out r.X);
				//        TypeConvert.FromString(v[1].Trim(), out r.Y);
				//        TypeConvert.FromString(v[2].Trim(), out r.Width);
				//        TypeConvert.FromString(v[3].Trim(), out r.Height);
				//        return r;
				//    }
				//}
				//catch { }

				//// Whoops. Parsing failed. Throw an error.
				//throw new ArgumentException(
				//    "Conversion from \"" + (string)value +
				//    "\" to a RectangleF failed. Format must be" +
				//    "\"X" + separator[0] + " Y" + separator[0] + " W" + separator[0] + " H\"");
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context,
		   CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is FilePath)
			{
				var v = (FilePath)value;
				return v.String;
			}
			
			if (destinationType == typeof(InstanceDescriptor))
			{
				if (value == null)
					return null;

				var s = (FilePath)value;

				MemberInfo Member;
				object[] Arguments;

				// get the constructor of our Latitude type
				Member = typeof(FilePath).GetConstructor(new Type[] { typeof(string) });

				// the arguments to pass along to the Latitude constructor
				Arguments = new object[] { s };

				if (Member != null)
					return new InstanceDescriptor(Member, Arguments);

				return null;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		

		//public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		//{
		//    if (propertyValues == null)
		//    {
		//        throw new ArgumentNullException("propertyValues", "Missing property values.");
		//    }
		//    return new RectangleF((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Width"], (float)propertyValues["Height"]);
		//}
	}
}