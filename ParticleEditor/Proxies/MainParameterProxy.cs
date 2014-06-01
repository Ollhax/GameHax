using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;

namespace MG.ParticleEditor.Proxies
{
	class MainParameterProxy : BaseParameterProxy
	{
		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return changeset.CurrentDefinition.Name; } set { changeset.CurrentDefinition.Name = value; } }

		public MainParameterProxy(Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
			: base(model, particleDeclaration, particleDefinition)
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
				var value = param.Value.Value;

				ParticleDeclaration.Parameter declarationParameter;
				if (particleDeclaration.Parameters.TryGetValue(param.Value.Name, out declarationParameter))
				{
					var p = new AnyPropertyDescriptor(declarationParameter, value);
					p.PropertyChanged += () => OnPropertyChanged(p);
					pdc.Add(p);
				}
			}
			
			return pdc;
		}

		private void OnPropertyChanged(AnyPropertyDescriptor property)
		{
			changeset.CurrentParameter = property.DeclarationParameter.Name;
		}
	}
}
