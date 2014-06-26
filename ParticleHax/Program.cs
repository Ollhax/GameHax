using MG.ParticleHax.Controllers;
using MG.ParticleEditorWindow;
using MonoDevelop.MacInterop;
using System;

namespace MG.ParticleHax
{
	class MainClass
	{
		private static MainController mainController;

		public static void Main(string[] args)
		{
			const string applicationName = "ParticleHax";

			Framework.Framework.Initialize("Main", applicationName);
			Application.Init(applicationName, args);

			var file = args.Length > 0 ? args[0] : "";
			
			using (mainController = new MainController())
			{
				if (!string.IsNullOrEmpty(file))
				{
					mainController.Open(file);
				}
				else
				{
					mainController.New();
				}

				if (Framework.Utility.Platform.IsMac)
				{
					ApplicationEvents.OpenDocuments += delegate(object sender, ApplicationDocumentEventArgs e)
					{
						if (mainController != null && e.Documents != null && e.Documents.Count > 0)
						{
							foreach (var d in e.Documents)
							{
								mainController.Open(d.Key);
								break;
							}
						}

						e.Handled = true;
					};
				}

				Application.Run();
			}

			Framework.Framework.Deinitialize();
		}
	}
}
