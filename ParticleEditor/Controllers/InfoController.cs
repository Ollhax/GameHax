using System;

using MG.EditorCommon;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class InfoController
	{
		private MainController controller;
		private Model model;
		private InfoView infoView;
		public SubParameterController SubParameterController;

		public InfoController(MainController controller, Model model, InfoView infoView)
		{
			this.controller = controller;
			this.model = model;
			this.infoView = infoView;

			SubParameterController = new SubParameterController(controller, model, infoView.MetaProperties);
		}
		
		public void UpdateInfo()
		{
			var currentParameter = model.CurrentParameter;
			SubParameterController.OnChangeParameter();
			
			var def = model.CurrentDefinition;
			if (def == null || string.IsNullOrEmpty(currentParameter))
			{
				infoView.Visible = false;
			}
			else
			{
				infoView.Visible = true;
				ParticleDeclaration declaration;
				if (model.DeclarationTable.Declarations.TryGetValue(def.Declaration, out declaration))
				{
					ParticleDeclaration.Parameter parameter;
					if (declaration.Parameters.TryGetValue(currentParameter, out parameter))
					{
						infoView.Name = parameter.PrettyName;
						infoView.Description = parameter.Description;
					}
				}
			}
		}
	}
}
