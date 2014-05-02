using System;

using Cairo;

using Gtk;
using Gdk;
using MG.Framework.Numerics;

namespace EditorCommon
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGraph : Gtk.ScrolledWindow// : Gtk.DrawingArea
	{
		private DrawingArea drawingArea;

		public HaxGraph()
		{
			//ScrolledWindow scroller = new ScrolledWindow();
			Viewport viewer = new Viewport();
			viewer.ShadowType = ShadowType.None;
			
			drawingArea = new DrawingArea();
			drawingArea.SetSizeRequest(200, 500);
			
			drawingArea.ExposeEvent += DrawingAreaOnExposeEvent;
			viewer.Add(drawingArea);

			Add(viewer);
			//scroller.Add(viewer);

			//AddEvents((int)(EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask));

			//var scrollXAdjust = new Adjustment(0, 0, 100, 1, 10, 10);
			//var scrollX = new HScrollbar(scrollXAdjust);
			////this.Add(scrollX);
		}

		private void DrawingAreaOnExposeEvent(object o, ExposeEventArgs args)
		{
			using (Cairo.Context c = Gdk.CairoHelper.Create(args.Event.Window))
			{
				var area = Allocation;
				var lg1 = new LinearGradient(0, 0, 0, area.Height);
				lg1.AddColorStop(0, new Cairo.Color(1, 0, 0));
				lg1.AddColorStop(area.Height, new Cairo.Color(0, 1, 0));

				c.Rectangle(args.Event.Region.Clipbox.Left, args.Event.Region.Clipbox.Top, area.Width, area.Height);
				//c.SetSourceRGBA(1, 0, 0, 1);
				c.SetSource(lg1);
				
				c.Fill();

				lg1.Dispose();
			}
		}

		protected override bool OnButtonPressEvent(EventButton evnt)
		{
			drawingArea.SetSizeRequest(200, 200);
			
			//Console.WriteLine(evnt.Button);
			return base.OnButtonPressEvent(evnt);
		}

		//protected override bool OnButtonReleaseEvent(EventButton evnt)
		//{
		//    return base.OnButtonReleaseEvent(evnt);
		//}

		//protected override bool OnMotionNotifyEvent(EventMotion evnt)
		//{
		//    var position = new Vector2((float)evnt.X, (float)evnt.Y);
		//    Console.WriteLine(position);
		//    return base.OnMotionNotifyEvent(evnt);
		//}
	}
}

