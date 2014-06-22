using MG.ParticleEditor.Controllers;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Framework.Framework.Initialize("Main", "");
			Application.Init("Particle Editor", args);

			var file = args.Length > 0 ? args[0] : "";
			
			using (var mainController = new MainController(file))
			{
				Application.Run();
			}

			Framework.Framework.Deinitialize();
		}
	}
}
