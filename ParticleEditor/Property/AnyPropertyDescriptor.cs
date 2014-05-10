using System;
using System.ComponentModel;

using MG.Framework.Utility;

namespace MG.ParticleEditor
{
	public class AnyPropertyDescriptor : PropertyDescriptor
	{
		private Any any;
		
		public readonly ParticleDeclaration.Parameter DeclarationParameter;

		public AnyPropertyDescriptor(ParticleDeclaration.Parameter declarationParameter, Any any)
			: base(declarationParameter.PrettyName, null)
		{
			DeclarationParameter = declarationParameter;
			this.any = any;
		}

		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return null; } }
		public override object GetValue(object component) { return any.GetAsObject(); }
		public override string Description { get { return DeclarationParameter.Description; } }
		public override string Category { get { return DeclarationParameter.Category; } }
		public override bool IsReadOnly { get { return false; } }
		public override void ResetValue(object component) { }
		public override bool ShouldSerializeValue(object component) { return false; }

		public override TypeConverter Converter
		{
			get
			{
				if (DeclarationParameter.ValueList != null)
				{
					return new ValueListConverter(DeclarationParameter.ValueList);
				}

				return base.Converter;
			}
		}

		public override void SetValue(object component, object value)
		{
			any.Set(value);
		}

		public override Type PropertyType
		{
			get { return any.GetTypeOfValue(); }
		}

		public override object GetEditor(Type editorBaseType)
		{
			if (any.IsInt())
				return new IntEditor();

			if (any.IsFloat())
				return new FloatEditor();

			return base.GetEditor(editorBaseType);
		}
	}
}
