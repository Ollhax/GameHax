using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class RectangleConverter : MathTypeConverter
	{
		// Methods
		public RectangleConverter()
		{
			Type type = typeof(Rectangle);
			PropertyDescriptorCollection descriptors =
				new PropertyDescriptorCollection(
					new PropertyDescriptor[]
						{
							new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")),
							new FieldPropertyDescriptor(type.GetField("Width")), new FieldPropertyDescriptor(type.GetField("Height"))
						});
			base.propertyDescriptions = descriptors;
			//base.supportStringConvert = false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			int[] numArray = MathTypeConverter.ConvertToValues<int>(context, culture, value, 4, new[] { "X", "Y", "Width", "Height" });
			if (numArray != null)
			{
				return new Rectangle(numArray[0], numArray[1], numArray[2], numArray[3]);
			}
			return base.ConvertFrom(context, culture, value);
		}
		
		public override object ConvertTo(
			ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if ((destinationType == typeof(string)) && (value is Rectangle))
			{
				var rect = (Rectangle)value;
				return MathTypeConverter.ConvertFromValues<int>(context, culture, new[] { rect.X, rect.Y, rect.Width, rect.Height });
			}
			if ((destinationType == typeof(InstanceDescriptor)) && (value is Rectangle))
			{
				Rectangle rectangle = (Rectangle)value;
				ConstructorInfo constructor =
					typeof(Rectangle).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
				if (constructor != null)
				{
					return new InstanceDescriptor(
						constructor, new object[] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues", "Null not allowed");
			}
			return new Rectangle(
				(int)propertyValues["X"], (int)propertyValues["Y"], (int)propertyValues["Width"], (int)propertyValues["Height"]);
		}
	}
}


