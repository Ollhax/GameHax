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
			
			using (var mainController = new MainController())
			{
				Application.Run();
			}

			Framework.Framework.Deinitialize();
		}
	}
}
