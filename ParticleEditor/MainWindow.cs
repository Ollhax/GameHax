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

		GLib.Timeout.Add(10, new GLib.TimeoutHandler(Refresh));
    }

	private bool Refresh()
	{

		glwidget1.QueueDraw();
		glwidget2.QueueDraw();
		// returning true means that the timeout routine should be invoked
		// again after the timeout period expires.   Returning false would
		// terminate the timeout.

		return true;
	}

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

	protected void OnGlwidget2RenderFrame(object sender, EventArgs e)
	{
		GraphicsDevice.Clear();
	}

	protected void OnGlwidget1RenderFrame(object sender, EventArgs e)
	{
		GraphicsDevice.Clear();
	}
}
