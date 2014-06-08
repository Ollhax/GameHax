using System;

using MG.EditorCommon.Undo;
using MG.ParticleEditor.Proxies;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	abstract class AbstractParameterController
	{
		protected MainController controller;
		protected Model model;
		protected ParameterView parameterView;
		protected BaseParameterProxy parameterProxy;

		public event Action<string> ParameterSelected = delegate { };
		public event Action StartedEdit = delegate { };

		public AbstractParameterController(MainController controller, Model model, ParameterView parameterView)
		{
			this.controller = controller;
			this.model = model;
			this.parameterView = parameterView;
			parameterView.Changed += OnParameterChanged;
			parameterView.EndedEdit += OnParameterEndedEdit;
			parameterView.SelectionChanged += OnParameterSelected;
			parameterView.StartedEdit += OnParameterStartedEdit;
			model.UndoHandler.UndoEvent += OnUndoRedoEvent;
			model.UndoHandler.RedoEvent += OnUndoRedoEvent;
		}
		
		public void SelectParameter(string parameter)
		{
			parameterView.SelectParameter(parameter);
		}

		public void CancelEdit()
		{
			parameterView.CancelChanges();
		}

		protected abstract void ReloadProxy();

		protected virtual void OnParameterSelected(string parameter)
		{
			ParameterSelected(parameter);
		}

		private void OnParameterStartedEdit()
		{
			StartedEdit();
		}

		private void OnParameterChanged()
		{
			if (parameterProxy != null)
			{
				model.UndoHandler.ExecuteAction(parameterProxy.CommitAction());
			}
		}

		private void OnParameterEndedEdit(bool cancelled)
		{
			if (!cancelled && parameterProxy != null)
			{
				model.UndoHandler.ExecuteAction(parameterProxy.CommitAction());
				model.UndoHandler.EndUndoGroup(parameterProxy.UndoGroup);
			}
		}

		private void OnUndoRedoEvent(IUndoableAction action)
		{
			parameterView.CancelChanges();
			ReloadProxy();
		}
	}
}
