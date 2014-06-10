using System;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class ParameterController
	{
		protected MainController controller;
		protected Model model;
		protected ParameterView parameterView;
		protected ParameterProxy parameterProxy;

		public event Action<string> ParameterSelected = delegate { };
		public event Action StartedEdit = delegate { };

		public ParameterController(MainController controller, Model model, ParameterView parameterView)
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

		public void OnChangeDefinition(ParticleDefinition definition)
		{
			parameterView.CommitChanges();
			ReloadProxy();
		}

		protected void ReloadProxy()
		{
			parameterProxy = null;

			var def = model.CurrentDefinition;
			if (def != null)
			{
				ParticleDeclaration particleDeclaration;
				if (model.DeclarationTable.Declarations.TryGetValue(def.Declaration, out particleDeclaration))
				{
					parameterProxy = new ParameterProxy(controller, model, particleDeclaration, def);
				}
			}

			parameterView.SetCurrentObject(parameterProxy);
		}

		protected void OnParameterSelected(string parameter)
		{
			model.CurrentParameter = parameter;
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
