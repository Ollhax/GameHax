using System;
using System.Reflection;

namespace MG.Framework.Converters
{
	internal class FieldPropertyDescriptor : MemberPropertyDescriptor
	{
		// Fields
		private FieldInfo _field;

		// Methods
		public FieldPropertyDescriptor(FieldInfo field)
			: base(field)
		{
			this._field = field;
		}

		public override object GetValue(object component)
		{
			return this._field.GetValue(component);
		}

		public override void SetValue(object component, object value)
		{
			this._field.SetValue(component, value);
			this.OnValueChanged(component, EventArgs.Empty);
		}

		// Properties
		public override Type PropertyType
		{
			get
			{
				return this._field.FieldType;
			}
		}
	}
}
