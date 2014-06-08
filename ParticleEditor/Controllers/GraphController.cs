using System;

using MG.ParticleEditor.Actions;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class GraphController
	{
		private MainController controller;
		private Model model;
		private GraphView graphView;

		private CurveChangeAction changeAction;

		public event Action GraphChanged = delegate { };

		public GraphController(MainController controller, Model model, GraphView graphView)
		{
			this.controller = controller;
			this.model = model;
			this.graphView = graphView;

			graphView.Changed += OnChanged;
		}

		public void OnSubParameterSelected(string subParameter)
		{
			changeAction = new CurveChangeAction(controller, model, model.CurrentDefinitionId, model.CurrentParameter, model.CurrentSubParameter);

			graphView.Curve = changeAction.Curve;
		}

		private void OnChanged()
		{
			changeAction.UpdateModel();
			GraphChanged();
		}
	}
}
