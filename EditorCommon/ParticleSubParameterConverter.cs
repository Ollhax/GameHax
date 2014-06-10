using System;
using System.ComponentModel;
using MG.Framework.Particle;

namespace MG.EditorCommon
{
	class ParticleSubParameterConverter : ExpandableObjectConverter
	{
		private PropertyDescriptorCollection propertyDescriptor;

		public ParticleSubParameterConverter(ParticleParameterDescriptor.PropertyChangeDelegate changeDelegate, ParticleDeclaration.Parameter declarationParameter, ParticleDefinition.Parameter definitionParameter)
		{
			propertyDescriptor = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

			foreach (var paramPair in declarationParameter.Parameters)
			{
				ParticleDeclaration.Parameter parameter = paramPair.Value;
				var descriptor = new ParticleParameterDescriptor(parameter, definitionParameter.Parameters[parameter.Name]);
				descriptor.PropertyChanged += delegate(string name) { changeDelegate(declarationParameter.Name + "." + name); };
				propertyDescriptor.Add(descriptor);
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
