using System;
using Gtk;
using MG.Framework.Graphics;

public partial class MainWindow: Gtk.Window
{
    const int id = 1;

    public MainWindow() : base (Gtk.WindowType.Toplevel)
    {
        Build();
		//GraphicsDevice graphicsDevice = new GraphicsDevice();

        statusbar5.Push(id, "Meep");
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

	protected void OnGlwidget2RenderFrame(object sender, EventArgs e)
	{
		//GraphicsDevice.Clear();
	}
}
