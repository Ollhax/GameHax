using System;
using Gtk;

using MG.Framework;

namespace ParticleEditor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Framework.Initialize("Main", "");
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
			Framework.Deinitialize();
		}
	}
}
