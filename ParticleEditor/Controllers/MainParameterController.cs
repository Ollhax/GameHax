using MG.EditorCommon;
using MG.Framework.Particle;
using MG.ParticleEditor.Proxies;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class MainParameterController : AbstractParameterController
	{
		public MainParameterController(MainController controller, Model model, ParameterView parameterView)
			: base(controller, model, parameterView)
		{
			
		}

		public void OnChangeDefinition(ParticleDefinition definition)
		{
			parameterView.CommitChanges();
			ReloadProxy();
		}

		protected override void OnParameterSelected(string parameter)
		{
			model.CurrentParameter = parameter;
			base.OnParameterSelected(parameter);
		}
		
		protected override void ReloadProxy()
		{
			parameterProxy = null;

			var def = model.CurrentDefinition;
			if (def != null)
			{
				ParticleDeclaration particleDeclaration;
				if (model.DeclarationTable.Declarations.TryGetValue(def.Declaration, out particleDeclaration))
				{
					parameterProxy = new MainParameterProxy(controller, model, particleDeclaration, def);
				}
			}

			parameterView.SetCurrentObject(parameterProxy);
		}
	}
}
