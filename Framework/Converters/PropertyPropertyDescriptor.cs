using System;
using System.Reflection;

namespace MG.Framework.Converters
{
	internal class PropertyPropertyDescriptor : MemberPropertyDescriptor
	{
		// Fields
		private PropertyInfo property;

		// Methods
		public PropertyPropertyDescriptor(PropertyInfo property)
			: base(property)
		{
			this.property = property;
		}

		public override object GetValue(object component)
		{
			return property.GetValue(component, null);
		}

		public override void SetValue(object component, object value)
		{
			property.SetValue(component, value, null);
			OnValueChanged(component, EventArgs.Empty);
		}

		// Properties
		public override Type PropertyType
		{
			get
			{
				return property.PropertyType;
			}
		}
	}
}
