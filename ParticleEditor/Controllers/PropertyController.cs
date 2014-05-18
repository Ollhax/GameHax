using System;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class PropertyController
	{
		private Model model;
		private PropertyView propertyView;
		private ParticlePropertyProxy particlePropertyProxy;
		
		public PropertyController(Model model, PropertyView propertyView)
		{
			this.model = model;
			this.propertyView = propertyView;
			propertyView.PropertyChanged += OnPropertyChanged;
			propertyView.Deselected += OnPropertyDeselected;
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}

		public void OnChangeDefinition(ParticleDefinition definition)
		{
			propertyView.CommitChanges();
			ReloadProxy();
		}

		private void ReloadProxy()
		{
			particlePropertyProxy = null;

			var def = model.CurrentDefinition;
			if (def != null)
			{
				ParticleDeclaration particleDeclaration;
				if (model.Declaration.Declarations.TryGetValue(def.Declaration, out particleDeclaration))
				{
					particlePropertyProxy = new ParticlePropertyProxy(particleDeclaration, def);
				}
			}

			propertyView.SetCurrentObject(particlePropertyProxy);
		}

		private void OnPropertyDeselected()
		{
			if (particlePropertyProxy != null)
			{
				model.UndoHandler.ExecuteAction(particlePropertyProxy.CommitAction());
				model.UndoHandler.EndUndoGroup(ParticlePropertyProxy.UndoGroup);
			}
		}

		private void OnPropertyChanged()
		{
			if (particlePropertyProxy != null)
			{
				model.UndoHandler.ExecuteAction(particlePropertyProxy.CommitAction());
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
		
		private void OnUndoRedoEvent(IUndoableAction action)
		{
			propertyView.CancelChanges();
			ReloadProxy();
		}
	}
}
