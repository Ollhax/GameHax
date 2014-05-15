using System;

using MG.EditorCommon;
using MG.Framework.Particle;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class PropertyController
	{
		private Model model;
		private PropertyView propertyView;

		public PropertyController(Model model, PropertyView propertyView)
		{
			this.model = model;
			this.propertyView = propertyView;
			propertyView.PropertyChanged += OnPropertyChanged;
		}
		
		public void OnItemSelected(ParticleDefinition definition)
		{
			ParticleDeclaration particleDeclaration;
			if (model.Declaration.Declarations.TryGetValue(definition.Declaration, out particleDeclaration))
			{
				propertyView.SetCurrentObject(new ParticlePropertyProxy(particleDeclaration, definition));
				
				//propertygrid1.Changed += Propertygrid1OnChanged;
			}

			//if (model.ParticleSystem == null || definition != model.ParticleSystem.Definition)
			//{
			//    model.ParticleSystem = new ParticleSystem(assetHandler, definition);
			//}
		}

		private void OnPropertyChanged()
		{
			if (model.ParticleSystem == null) 
				return;

			model.ParticleSystem.Reload();
		}
	}
}
