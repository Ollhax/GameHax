using System;
using System.ComponentModel;
using MG.EditorCommon;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Proxies
{
	class SubParameterProxy : BaseParameterProxy
	{
		private string parameterName;

		public SubParameterProxy(MainController controller, Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition, string parameterName)
			: base(controller, model, particleDeclaration, particleDefinition, 112515)
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
					var definitionParam = param.Value;
					ParticleDeclaration.Parameter declarationSubParameter;
					if (declarationParameter.Parameters.TryGetValue(definitionParam.Name, out declarationSubParameter))
					{
						var p = new ParticleParameterDescriptor(declarationSubParameter, definitionParam);
						//p.PropertyChanged += () => OnPropertyChanged(p);
						pdc.Add(p);
					}
				}
			}

			return pdc;
		}
		
		//private void OnPropertyChanged(ParticleParameterDescriptor property)
		//{
		//    changeset.CurrentSubParameter = property.DeclarationParameter.Name;
		//}
	}
}
