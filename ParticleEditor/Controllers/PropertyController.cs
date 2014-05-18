using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class PropertyController
	{
		private Model model;
		private PropertyView propertyView;
		private UndoableAction currentProxy;
		private bool shouldUpdateProxy;

		public PropertyController(Model model, PropertyView propertyView)
		{
			this.model = model;
			this.propertyView = propertyView;
			propertyView.PropertyChanged += OnPropertyChanged;
			propertyView.Deselected += OnPropertyDeselected;
			model.UndoHandler.AfterStateChanged += OnAfterStateChanged;
		}
		
		public void Update()
		{
			if (shouldUpdateProxy)
			{
				shouldUpdateProxy = false;
				UpdateParticleSystem();
				ReloadProxy();
			}
		}

		public void OnItemSelected(ParticleDefinition definition)
		{
			ReloadProxy();
		}

		private void ReloadProxy()
		{
			currentProxy = null;

			var def = model.CurrentDefinition;
			if (def != null)
			{
				ParticleDeclaration particleDeclaration;
				if (model.Declaration.Declarations.TryGetValue(def.Declaration, out particleDeclaration))
				{
					currentProxy = new ParticlePropertyProxy(particleDeclaration, def);
				}
			}

			propertyView.SetCurrentObject(currentProxy);
		}

		private void OnPropertyChanged()
		{
			if (currentProxy != null)
			{
				model.UndoHandler.ExecuteAction(currentProxy);
			}

			UpdateParticleSystem();
		}

		private void UpdateParticleSystem()
		{
			if (model.ParticleSystem != null)
			{
				model.ParticleSystem.Reload();
			}
		}
		
		private void OnPropertyDeselected()
		{
			if (currentProxy != null)
			{
				model.UndoHandler.EndUndoGroup(currentProxy.GetUndoGroup());
			}
		}
		
		private void OnAfterStateChanged()
		{
			shouldUpdateProxy = true;
		}
	}
}
