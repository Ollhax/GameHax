using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class Vector2Converter : MathTypeConverter
	{
		// Methods
		public Vector2Converter()
		{
			Type type = typeof(Vector2);
			base.propertyDescriptions =
				new PropertyDescriptorCollection(
					new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")) }).Sort(
							new[] { "X", "Y" });
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			float[] numArray = MathTypeConverter.ConvertToValues<float>(context, culture, value, 2, new[] { "X", "Y" });
			if (numArray != null)
			{
				return new Vector2(numArray[0], numArray[1]);
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
			if ((destinationType == typeof(string)) && (value is Vector2))
			{
				var vector2 = (Vector2)value;
				return MathTypeConverter.ConvertFromValues<float>(context, culture, new[] { vector2.X, vector2.Y });
			}
			if ((destinationType == typeof(InstanceDescriptor)) && (value is Vector2))
			{
				var vector = (Vector2)value;
				ConstructorInfo constructor = typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) });
				if (constructor != null)
				{
					return new InstanceDescriptor(constructor, new object[] { vector.X, vector.Y });
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
			return new Vector2((float)propertyValues["X"], (float)propertyValues["Y"]);
		}
	}
}