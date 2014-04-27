using System;
using Gtk;

using MG.Framework;

namespace ParticleEditor
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Framework.Initialize();
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
			Framework.Deinitialize();
		}
	}
}
