using System;
using System.ComponentModel;

using MG.EditorCommon.Editors;
using MG.Framework.Particle;

namespace MG.EditorCommon
{
	public class ParticleParameterDescriptor : PropertyDescriptor
	{
		public ParticleParameterDescriptor(ParticleDeclaration.Parameter declarationParameter, ParticleDefinition.Parameter definitionParameter)
			: base(declarationParameter.Name, null)
		{
			DeclarationParameter = declarationParameter;
			DefinitionParameter = definitionParameter;
		}
		
		public readonly ParticleDeclaration.Parameter DeclarationParameter;
		public readonly ParticleDefinition.Parameter DefinitionParameter;
		public delegate void PropertyChangeDelegate(string propertyName);
		public event PropertyChangeDelegate PropertyChanged;
		
		public override string DisplayName { get { return DeclarationParameter.PrettyName; } }
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return null; } }
		public override object GetValue(object component) { return DefinitionParameter.Value.GetAsObject(); }
		public override string Description { get { return DeclarationParameter.Description; } }
		public override string Category { get { return DeclarationParameter.Category; } }
		public override bool IsReadOnly { get { return false; } }
		public override void ResetValue(object component) { }
		public override bool ShouldSerializeValue(object component) { return false; }

		public override TypeConverter Converter
		{
			get
			{
				if (DeclarationParameter.Parameters.Count > 0)
				{
					return new ParticleSubParameterConverter(PropertyChanged, DeclarationParameter, DefinitionParameter);
				}

				if (DeclarationParameter.ValueList != null)
				{
					return new ValueListConverter(DeclarationParameter.ValueList);
				}

				if (base.Converter is ExpandableObjectConverter)
				{
					// Don't want anything but subparameters to expand, so create wrapper converters for expandable types
				    return new NonExpandableObjectConverter(base.Converter);
				}

				return base.Converter;
			}
		}

		public override void SetValue(object component, object value)
		{
			DefinitionParameter.Value.Set(value);
			if (PropertyChanged != null)
			{
				PropertyChanged.Invoke(DeclarationParameter.Name);
			}
		}

		public override Type PropertyType
		{
			get { return DefinitionParameter.Value.GetTypeOfValue(); }
		}

		public override object GetEditor(Type editorBaseType)
		{
			if (DefinitionParameter.Value.IsInt()) return new IntEditor();
			if (DefinitionParameter.Value.IsFloat()) return new FloatEditor();
			if (DefinitionParameter.Value.IsFilePath()) return new FilePathEditor();
			if (DefinitionParameter.Value.IsCurve()) return new GraphEditor();
			if (DefinitionParameter.Value.IsGradient()) return new GradientEditor();
			if (DefinitionParameter.Value.IsNoise()) return new NoiseEditor();

			return base.GetEditor(editorBaseType);
		}
	}
}
