using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class RelativePosition2Converter : MathTypeConverter
	{
		// Methods
		public RelativePosition2Converter()
		{
			Type type = typeof(RelativePosition2);
			base.propertyDescriptions =
				new PropertyDescriptorCollection(
					new PropertyDescriptor[]
						{
							new FieldPropertyDescriptor(type.GetField("OffsetX")), 
							new FieldPropertyDescriptor(type.GetField("OffsetY")),
							new FieldPropertyDescriptor(type.GetField("RelativeX")),
							new FieldPropertyDescriptor(type.GetField("RelativeY"))
						}).Sort(
							new[] { "OffsetX", "OffsetY", "RelativeX", "RelativeY" });
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			float[] numArray = ConvertToValues<float>(context, culture, value, 4, new[] { "OffsetX", "OffsetY", "RelativeX", "RelativeY" });
			if (numArray != null)
			{
				return new RelativePosition2(numArray[0], numArray[1], numArray[2], numArray[3]);
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
			if ((destinationType == typeof(string)) && (value is RelativePosition2))
			{
				var position2 = (RelativePosition2)value;
				return ConvertFromValues(context, culture, new[] { position2.OffsetX, position2.OffsetY, position2.RelativeX, position2.RelativeY });
			}
			if ((destinationType == typeof(InstanceDescriptor)) && (value is RelativePosition2))
			{
				var position2 = (RelativePosition2)value;
				ConstructorInfo constructor = typeof(RelativePosition2).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) });
				if (constructor != null)
				{
					return new InstanceDescriptor(constructor, new object[] { position2.OffsetX, position2.OffsetY, position2.RelativeX, position2.RelativeY });
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
			return new RelativePosition2((float)propertyValues["OffsetX"], (float)propertyValues["OffsetY"], (float)propertyValues["RelativeX"], (float)propertyValues["RelativeY"]);
		}
	}
}