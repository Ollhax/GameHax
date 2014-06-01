using System;
using System.ComponentModel;
using MG.EditorCommon;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;

namespace MG.ParticleEditor.Proxies
{
	class SubParameterProxy : BaseParameterProxy
	{
		private string parameterName;

		public SubParameterProxy(Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition, string parameterName)
			: base(model, particleDeclaration, particleDefinition)
		{
			this.parameterName = parameterName;
			changeset.CurrentParameter = parameterName;
		}
		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			var typeProperties = TypeDescriptor.GetProperties(this.GetType(), attributes);
			foreach (PropertyDescriptor pd in typeProperties)
			{
				pdc.Add(pd);
			}
			
			ParticleDeclaration.Parameter declarationParameter;
			ParticleDefinition.Parameter parameter;
			if (changeset.CurrentDefinition.Parameters.TryGetValue(parameterName, out parameter) &&
				particleDeclaration.Parameters.TryGetValue(parameter.Name, out declarationParameter))
			{
				foreach (var param in parameter.Parameters)
				{
					var value = param.Value.Value;
					ParticleDeclaration.Parameter declarationSubParameter;
					if (declarationParameter.Parameters.TryGetValue(param.Value.Name, out declarationSubParameter))
					{
						pdc.Add(new AnyPropertyDescriptor(declarationSubParameter, value));
					}
				}
			}

			return pdc;
		}
	}
}
