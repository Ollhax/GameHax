using System;

namespace MG.ParticleEditorWindow
{
	public static class Application
	{
		public static event Action Update;

		public static void Init(string title, string[] args)
		{
			Gtk.Application.Init(title, ref args);
			GLib.Timeout.Add(33, OnUpdate);
		}

		public static void Run()
		{
			Gtk.Application.Run();
		}

		public static void Quit()
		{
			Gtk.Application.Quit();
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
