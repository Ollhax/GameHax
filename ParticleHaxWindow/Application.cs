using System;
using GLib;
using MG.Framework.Utility;

namespace MG.ParticleEditorWindow
{
	public static class Application
	{
		public static event Action Update;

		public static void Init(string title, string[] args)
		{
			Gtk.Application.Init(title, ref args);			
			GLib.Timeout.Add(33, OnUpdate);

			UnhandledExceptionHandler h = new UnhandledExceptionHandler (OnException);
			ExceptionManager.UnhandledException += h;
		}

		public static void Run()
		{
			Gtk.Application.Run();
		}

		public static void Quit()
		{
			Gtk.Application.Quit();
		}

		private static void OnException(UnhandledExceptionArgs args)
		{
			ExceptionHandler.RaiseException((Exception)args.ExceptionObject, false);
			//ShowErrorDialog(args.ExceptionObject, args.IsTerminating);
			//args.ExitApplication = true;
			//throw args.ExceptionObject;
		}

		private static bool OnUpdate()
		{
			if (Update != null)
			{
				Update.Invoke();
			}
			return true;
		}
	}
}
