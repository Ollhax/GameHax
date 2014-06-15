using MG.Framework.Numerics;
using MG.Framework.Particle;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor.Actions
{
	class CurveChangeAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		
		public Curve Curve;
		private Curve oldCurve;
		private Curve newCurve;

		public CurveChangeAction(MainController controller, Model model, int definitionId, string currentParameter, string currentSubParameter)
		{
			this.controller = controller;
			this.model = model;
			CurrentDefinitionId = definitionId;
			CurrentParameter = currentParameter;
			
			var graphParameter = GetGraphParameter();
			if (graphParameter != null)
			{
				Curve = Curve.FromInvariantString(graphParameter.Value.Get<string>());
				oldCurve = new Curve(Curve);
			}
		}
		
		public void UpdateModel()
		{
			var graphParameter = GetGraphParameter();
			if (graphParameter == null) return;

			graphParameter.Value.Set(Curve.ToInvariantString());
			controller.UpdateParticleSystem = true;
		}

		protected override bool CallExecute()
		{
			if (Curve == null) return false;
			var graphParameter = GetGraphParameter();
			if (graphParameter == null) return false;
			
			graphParameter.Value.Set(Curve.ToString());
			newCurve = new Curve(Curve);
			return true;
		}

		protected override void CallUndo()
		{
			//var collection = model.DefinitionTable.Definitions.GetParentCollection(addedDefinition.Id);

			//if (collection != null)
			//{
			//    model.Modified = true;
			//    collection.Remove(addedDefinition);
			//    controller.UpdateTree = true;
			//}
		}
		
		private ParticleDefinition.Parameter GetGraphParameter()
		{
			//var def = model.DefinitionTable.Definitions.GetById(CurrentDefinitionId);

			//if (def != null && CurrentSubParameter != null && CurrentSubParameter.StartsWith("Graph"))
			//{
			//    ParticleDefinition.Parameter parameter;
			//    if (def.Parameters.TryGetValue(CurrentParameter, out parameter))
			//    {
			//        ParticleDefinition.Parameter graphParameter;
			//        if (parameter.Parameters.TryGetValue(CurrentSubParameter, out graphParameter))
			//        {
			//            return graphParameter;
			//        }
			//    }
			//}

			return null;
		}
	}
}
