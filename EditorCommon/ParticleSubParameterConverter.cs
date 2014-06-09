using System;
using System.ComponentModel;
using MG.Framework.Particle;

namespace MG.EditorCommon
{
	class ParticleSubParameterConverter : ExpandableObjectConverter
	{
		private PropertyDescriptorCollection propertyDescriptor;

		public ParticleSubParameterConverter(ParticleDeclaration.Parameter declarationParameter, ParticleDefinition.Parameter definitionParameter)
		{
			propertyDescriptor = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

			foreach (var paramPair in declarationParameter.Parameters)
			{
				ParticleDeclaration.Parameter parameter = paramPair.Value;
				propertyDescriptor.Add(new ParticleParameterDescriptor(parameter, definitionParameter.Parameters[parameter.Name]));
			}
		}
		
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return propertyDescriptor;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
