using MG.ParticleHax.Controllers;
using MG.ParticleEditorWindow;
using MonoDevelop.MacInterop;
using System;

namespace MG.ParticleHax
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			const string applicationName = "ParticleHax";

			Framework.Framework.Initialize("Main", applicationName);
			Application.Init(applicationName, args);

			var file = args.Length > 0 ? args[0] : "";
			
			using (var mainController = new MainController())
			{
				if (!string.IsNullOrEmpty(file))
				{
					mainController.DocumentController.Open(file);
				}
				else
				{
					mainController.DocumentController.New();
				}

				if (Framework.Utility.Platform.IsMac)
				{
					ApplicationEvents.OpenDocuments += delegate(object sender, ApplicationDocumentEventArgs e)
					{
						if (mainController != null && e.Documents != null && e.Documents.Count > 0)
						{
							foreach (var d in e.Documents)
							{
								mainController.DocumentController.Open(d.Key);
								break;
							}
						}

						e.Handled = true;
					};

					ApplicationEvents.Quit += delegate(object sender, ApplicationQuitEventArgs e)
					{
						if (mainController != null)
						{
							e.UserCancelled = !mainController.DocumentController.Close();
						}
					};
				}

				Application.Run();
			}

			Framework.Framework.Deinitialize();
		}
	}
}
