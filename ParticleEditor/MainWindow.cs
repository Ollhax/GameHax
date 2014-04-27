using System;
using Gtk;
using MG.Framework.Graphics;
using MG.Framework.Numerics;

public partial class MainWindow: Gtk.Window
{
	const int id = 1;
	private Texture2D texture;
	
	public MainWindow() : base (Gtk.WindowType.Toplevel)
	{
		Build();
		
		MainGL.Draw += MainGlOnDraw;
		MainGL.Load += MainGlOnLoad;

		haxglwidget1.Draw += MainGlOnDraw;
		haxglwidget1.Load += MainGlOnLoad;

		statusbar5.Push(id, "Meep");
		
		GLib.Timeout.Add(10, Refresh);
	}

	private void MainGlOnLoad()
	{
		texture = new Texture2D("weapon_laser_red.png");
	}

	private void MainGlOnDraw(RenderContext renderContext)
	{
		GraphicsDevice.ClearColor = Color.CornflowerBlue;
		GraphicsDevice.Clear();

		var screenSize = renderContext.ActiveScreen.ScreenSize;
		GraphicsDevice.SetViewport(new MG.Framework.Graphics.Viewport(0, 0, screenSize.X, screenSize.Y), renderContext.ActiveScreen);

		renderContext.QuadBatch.Begin();
		renderContext.QuadBatch.Draw(texture, Vector2.Zero);
		renderContext.QuadBatch.End();

		//renderContext.PrimitiveBatch.Begin();
		//renderContext.PrimitiveBatch.DrawFilled(new Circle(0, 0, 10000), Color.Red);
		//renderContext.PrimitiveBatch.End();		
	}

	private bool Refresh()
	{
		MainGL.QueueDraw();
		//haxglwidget2.QueueDraw();



		// again after the timeout period expires.   Returning false would
		// terminate the timeout.

		return true;
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	//protected void OnGlwidget2RenderFrame(object sender, EventArgs e)
	//{
	//    GraphicsDevice.Clear();
	//}

	//protected void OnGlwidget1RenderFrame(object sender, EventArgs e)
	//{
	//    GraphicsDevice.Clear();
	//}
}
