using MG.EditorCommon;
using MG.Framework.Particle;
using MG.ParticleEditor.Proxies;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class SubParameterController : AbstractParameterController
	{
		public SubParameterController(MainController controller, Model model, ParameterView parameterView)
			: base(controller, model, parameterView)
		{

		}

		public void OnChangeParameter()
		{
			parameterView.CommitChanges();
			ReloadProxy();
		}

		protected override void OnParameterSelected(string parameter)
		{
			model.CurrentSubParameter = parameter;
			base.OnParameterSelected(parameter);
		}

		protected override void ReloadProxy()
		{
			parameterProxy = null;

			var param = model.CurrentParameter;
			var def = model.CurrentDefinition;
			if (!string.IsNullOrEmpty(param) && def != null)
			{
				ParticleDeclaration particleDeclaration;
				if (model.DeclarationTable.Declarations.TryGetValue(def.Declaration, out particleDeclaration))
				{
					parameterProxy = new SubParameterProxy(controller, model, particleDeclaration, def, param);
				}
			}

			parameterView.SetCurrentObject(parameterProxy);
		}
	}
}
