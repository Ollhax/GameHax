using System;
using System.ComponentModel;
using System.Globalization;

namespace MG.EditorCommon
{
	class NonExpandableObjectConverter : TypeConverter
	{
		private TypeConverter expandableTypeConverter;

		public NonExpandableObjectConverter(TypeConverter expandableTypeConverter)
		{
			this.expandableTypeConverter = expandableTypeConverter;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return expandableTypeConverter.CanConvertFrom(context, sourceType);
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return expandableTypeConverter.ConvertFrom(context, culture, value);
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return expandableTypeConverter.ConvertTo(context, culture, value, destinationType);
		}
	}
}
