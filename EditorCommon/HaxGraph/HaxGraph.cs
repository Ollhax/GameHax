using System;
using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;

namespace MG.EditorCommon.HaxGraph
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGraph : ScrolledWindow
	{
		private readonly DrawingArea drawingArea;
		private ComplexCurve curve;

		public event Action Changed = delegate { };
		
		public ComplexCurve Curve
		{
			get { return curve; }

			set
			{
				if (curve == value) return;
				curve = value;

				QueueDraw();
			}
		}

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

			drawingArea.AddEvents((int)(EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask));
			drawingArea.MotionNotifyEvent += OnMotionNotifyEvent;
			
			//var scrollXAdjust = new Adjustment(0, 0, 100, 1, 10, 10);
			//var scrollX = new HScrollbar(scrollXAdjust);
			////this.Add(scrollX);
		}
		
		private void DrawingAreaOnExposeEvent(object o, ExposeEventArgs args)
		{
			using (Cairo.Context c = Gdk.CairoHelper.Create(args.Event.Window))
			{
				//var lg1 = new LinearGradient(0, 0, 0, area.Height);
				//lg1.AddColorStop(0, new Cairo.Color(1, 0, 0));
				//lg1.AddColorStop(area.Height, new Cairo.Color(0, 1, 0));

				//c.Rectangle(args.Event.Region.Clipbox.Left, args.Event.Region.Clipbox.Top, area.Width, area.Height);
				////c.SetSourceRGBA(1, 0, 0, 1);
				//c.SetSource(lg1);
				
				//c.Fill();

				//lg1.Dispose();



				//c.Rectangle(0, 0, area.Width, area.Height);
				//c.SetSourceRGBA(1, 1, 1, 1);
				//c.Fill();

				if (curve != null)
				{
					c.SetSourceRGBA(1, 0, 0, 1);
					
					foreach (var entry in curve)
					{
						var p = ToScreen(entry);
						c.LineTo(p.X, p.Y);
					}

					c.LineWidth = 1.0;
					c.Stroke();
				}
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

		private RectangleF GraphArea
		{
			get
			{
				var area = Allocation;
				return new RectangleF(area.X, area.Y, area.Width * 0.9f, area.Height);
			}
		}

		private Vector2 ToScreen(CurveEntry entry)
		{
			var area = GraphArea;
			return new Vector2(entry.Value.X * area.Width, area.Height - entry.Value.Y * area.Height);
		}

		private Vector2 FromScreen(Vector2 screenPoint)
		{
			var area = GraphArea;
			return new Vector2(screenPoint.X / area.Width, 1.0f - screenPoint.Y / area.Height);
		}

		private void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			var mousePos = new Vector2((float)args.Event.X, (float)args.Event.Y);
			
			if (curve != null)
			{
				CurveEntry currentEntry = null;
				foreach (var entry in curve)
				{
					var screenPoint = ToScreen(entry);
					
					float length = (screenPoint - mousePos).Length();
					
					if (length < 10)
					{
						currentEntry = entry;
						break;
					}
				}

				if (currentEntry != null)
				{
					var replacement = new CurveEntry(FromScreen(mousePos));
					curve.Remove(currentEntry);
					curve.Add(replacement);
					QueueDraw();

					Changed.Invoke();
				}
			}
		}
	}
}

