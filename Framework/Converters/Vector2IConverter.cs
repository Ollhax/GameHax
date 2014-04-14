using System;
using MG.Framework.Numerics;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;

namespace MG.Framework.Converters
{
	public class Vector2IConverter : ExpandableObjectConverter
	{
		private PropertyDescriptorCollection propertyDescriptor;

		public Vector2IConverter()
		{
			var type = typeof (Vector2I);
			propertyDescriptor = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")) }).Sort(new string[] { "X", "Y" });
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
				var separator = new string[] {culture.TextInfo.ListSeparator};

				try
				{
					var v = ((string) value).Split(separator, StringSplitOptions.None);
					if (v.Length == 2)
					{
						return new Vector2I(int.Parse(v[0]), int.Parse(v[1]));
					}
				}
				catch { }

				// Whoops. Parsing failed. Throw an error.
				throw new ArgumentException(
					"Conversion from \"" + (string)value +
					"\" to a Vector2I failed. Format must be" +
					"\"X" + separator[0] + " Y\"");
			}
			return base.ConvertFrom(context, culture, value);
		}
		
		public override object ConvertTo(ITypeDescriptorContext context,
		   CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is Vector2I)
			{
				var v = (Vector2I) value;
				var separator = culture.TextInfo.ListSeparator + " ";
				return (v.X + separator + v.Y);
			}
			else if (destinationType == typeof(InstanceDescriptor))
			{				
				if (value == null)
					return null;

				var vector = (Vector2I)value;

				MemberInfo Member;
				object[] Arguments;

				// get the constructor of our Latitude type
				Member = typeof(Vector2I).GetConstructor(new Type[] { typeof(int), typeof(int) });

				// the arguments to pass along to the Latitude constructor
				Arguments = new object[] { vector.X, vector.Y };
								
				if (Member != null)
					return new InstanceDescriptor(Member, Arguments);
				else
					return null;
			}

			return base.ConvertTo(context, culture, value, destinationType);
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

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues", "Missing property values.");
			}
			return new Vector2I((int)propertyValues["X"], (int)propertyValues["Y"]);
		}
	}
}
