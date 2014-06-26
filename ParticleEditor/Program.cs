using MG.ParticleEditor.Controllers;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			const string applicationName = "ParticleHax";

			Framework.Framework.Initialize("Main", applicationName);
			Application.Init(applicationName, args);

			var file = args.Length > 0 ? args[0] : "";
			
			using (var mainController = new MainController(file))
			{
				Application.Run();
			}

			Framework.Framework.Deinitialize();
		}
	}
}
