using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Proxies
{
	class MainParameterProxy : BaseParameterProxy
	{
		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return changeset.CurrentDefinition.Name; } set { changeset.CurrentDefinition.Name = value; } }

		public MainParameterProxy(MainController controller, Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
			: base(controller, model, particleDeclaration, particleDefinition, 754784)
		{
			
		}
		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			var typeProperties = TypeDescriptor.GetProperties(this.GetType(), attributes);
			foreach (PropertyDescriptor pd in typeProperties)
			{
				pdc.Add(pd);
			}

			foreach (var param in changeset.CurrentDefinition.Parameters)
			{
				var definitionParam = param.Value;

				ParticleDeclaration.Parameter declarationParameter;
				if (particleDeclaration.Parameters.TryGetValue(definitionParam.Name, out declarationParameter))
				{
					var p = new ParticleParameterDescriptor(declarationParameter, definitionParam);
					p.PropertyChanged += () => OnPropertyChanged(p);
					pdc.Add(p);
				}
			}
			
			return pdc;
		}

		private void OnPropertyChanged(ParticleParameterDescriptor property)
		{
			changeset.CurrentParameter = property.DeclarationParameter.Name;
		}
	}
}
