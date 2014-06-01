using System;

using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.ParticleEditor.Proxies;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	abstract class AbstractParameterController
	{
		protected Model model;
		protected ParameterView parameterView;
		protected BaseParameterProxy parameterProxy;

		public event Action<string> ParameterSelected = delegate { };
		
		public AbstractParameterController(Model model, ParameterView parameterView)
		{
			this.model = model;
			this.parameterView = parameterView;
			parameterView.Changed += OnParameterChanged;
			parameterView.EndedEdit += OnParameterEndedEdit;
			parameterView.SelectionChanged += s => ParameterSelected(s);
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}

		public void SelectParameter(string parameter)
		{
			parameterView.SelectParameter(parameter);
		}

		protected abstract void ReloadProxy();

		private void OnParameterChanged()
		{
			if (parameterProxy != null)
			{
				model.UndoHandler.ExecuteAction(parameterProxy.CommitAction());
			}

			UpdateParticleSystem();
		}
		
		private void OnParameterEndedEdit(bool cancelled)
		{
			if (!cancelled && parameterProxy != null)
			{
				model.UndoHandler.ExecuteAction(parameterProxy.CommitAction());
				model.UndoHandler.EndUndoGroup(BaseParameterProxy.UndoGroup);
			}
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
			parameterView.CancelChanges();
			ReloadProxy();
		}
	}
}
