using System;
using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;

namespace MG.EditorCommon.HaxGraph
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGraph : EventBox
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
			//Viewport viewer = new Viewport();
			//viewer.ShadowType = ShadowType.None;
			
			drawingArea = new DrawingArea();
			drawingArea.SetSizeRequest(-1, -1);
			//drawingArea.SetSizeRequest(200, 500);
			
			drawingArea.ExposeEvent += DrawingAreaOnExposeEvent;
			//viewer.Add(drawingArea);

			//Add(viewer);
			//scroller.Add(viewer);

			Add(drawingArea);

			drawingArea.AddEvents((int)(EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask));
			drawingArea.MotionNotifyEvent += OnMotionNotifyEvent;
			
			//var scrollXAdjust = new Adjustment(0, 0, 100, 1, 10, 10);
			//var scrollX = new HScrollbar(scrollXAdjust);
			////this.Add(scrollX);
		}
		
		public void Draw(Gdk.Drawable window, Cairo.Context ctx, Gdk.Rectangle bounds, StateType state)
		{
			if (curve == null)
				return;

			ctx.Save();
			ctx.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			ctx.Clip();

			var drawBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			
			if (state == StateType.Selected)
			{
				ctx.SetSourceRGBA(0.8, 0.8, 1.0, 1);
			}
			else
			{
				ctx.SetSourceRGBA(0.8, 0.8, 0.8, 1);
			}
			
			ctx.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			ctx.Fill();

			if (curve.Count > 0)
			{
				ctx.SetSourceRGBA(1, 0, 0, 1);
				
				int numSteps = (int)drawBounds.Width;

				var p = Evaluate(0, drawBounds);
				var l = p;
				ctx.MoveTo(p.X, p.Y);
				ctx.LineWidth = 1.0;
				SetDash(ctx, true);

				var startFraction = curve.Front.Value.X;
				var endFraction = curve.End.Value.X;
				int part = 0;
				
				for (int i = 0; i < numSteps; i++)
				{
					var fraction = (float)i / numSteps;
															
					p = Evaluate(fraction, drawBounds);
					ctx.LineTo(p.X, p.Y);
					l = p;

					bool pastStart = (fraction >= startFraction && part == 0);
					bool pastEnd = (fraction >= endFraction && part == 1);
					if (pastStart || pastEnd)
					{
						part++;						
						ctx.Stroke();
						SetDash(ctx, pastEnd);
						ctx.MoveTo(l.X, l.Y);
					}
				}

				ctx.Stroke();
			}

			ctx.Restore();
		}
		
		private void SetDash(Cairo.Context ctx, bool enabled)
		{
			if (enabled)
			{
				ctx.SetDash(new double[] { 4, 4 }, 1);
				ctx.LineCap = Cairo.LineCap.Butt;
			}
			else
			{
				ctx.SetDash(new double[] { }, 0);
			}
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

				Draw(drawingArea.ParentWindow, c, args.Event.Area, StateType.Selected);
			}
		}

		protected override bool OnButtonPressEvent(EventButton evnt)
		{
			//var point = new Vector2((float)evnt.X, (float)evnt.Y);
			//var area = GraphArea;
			//if (curve != null)
			//{
			//    foreach (var entry in curve)
			//    {
			//        var entryScreenPoint = ToScreen(entry, area);
			//        if ((entryScreenPoint - point).Length() < 10)
			//        {
			//            Console.WriteLine(entry.Value.X);
			//        }
			//    }
			//}

			//drawingArea.SetSizeRequest(200, 200);
			
			//Console.WriteLine(evnt.Button);
			//evnt.

			//return base.OnButtonPressEvent(evnt);
			return true;
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
				return new RectangleF(area.X, area.Y, area.Width, area.Height);
			}
		}
		
		private Vector2 Evaluate(float x, RectangleF area)
		{
			var y = curve.Evaluate(x);
			return ToScreen(new Vector2(x, y), area);
		}

		private Vector2 ToScreen(Vector2 value, RectangleF area)
		{
			return new Vector2(area.X + value.X * area.Width, area.Y + area.Height - value.Y * area.Height);
		}

		private Vector2 FromScreen(Vector2 screenPoint, RectangleF area)
		{
			return new Vector2((screenPoint.X - area.X) / area.Width, 1.0f - (screenPoint.Y - area.Y) / area.Height);
		}

		private void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			var area = GraphArea;
			args.RetVal = true; // Don't delegate this event to our parent
			var mousePos = new Vector2((float)args.Event.X, (float)args.Event.Y) + area.Position;
			
			if (curve != null)
			{
				CurveEntry currentEntry = null;
				foreach (var entry in curve)
				{
					var screenPoint = ToScreen(entry.Value, area);
					
					float length = (screenPoint - mousePos).Length();
					
					if (length < 10)
					{
						currentEntry = entry;
						break;
					}
				}

				if (currentEntry != null)
				{
					var replacement = new CurveEntry(FromScreen(mousePos, area));
					curve.Remove(currentEntry);
					curve.Add(replacement);
					QueueDraw();

					Changed.Invoke();
				}
			}
		}
	}
}

