using System;

using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;
using Key = Gdk.Key;

namespace MG.EditorCommon.HaxGraph
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGraph : DrawingArea
	{
		private readonly DrawingArea drawingArea;
		private ComplexCurve curve;
		private CurveEntry currentEntry;
		private bool movingEntry;
		
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
			ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255)); // Disable graying on selection
			AddEvents((int)(EventMask.AllEventsMask));
		}
		
		public void Draw(Gdk.Drawable window, Cairo.Context ctx, Gdk.Rectangle bounds, StateType state)
		{
			if (curve == null)
				return;
			
			var drawBounds = GraphAreaFromBounds(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
			var outerBounds = drawBounds.Inflated(3);
			
			// Outer clip area
			ctx.Save();
			ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
			ctx.Clip();

			var colorSelectedBackground = new MG.Framework.Numerics.Color(240, 240, 255, 255);
			var colorBorder = new MG.Framework.Numerics.Color(204, 204, 204, 255);
			var colorLine = new MG.Framework.Numerics.Color(255, 0, 0, 255);
			var colorHoveredEntry = new MG.Framework.Numerics.Color(0, 240, 0, 255);
			var colorSelectedEntry = new MG.Framework.Numerics.Color(0, 255, 0, 255);

			// Background
			if (state == StateType.Selected)
			{
				SetColor(ctx, colorSelectedBackground);
				ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
				ctx.Fill();
			}
			
			// Border
			SetColor(ctx, colorBorder);
			ctx.LineWidth = 1.0;
			ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
			ctx.Stroke();
			
			//// Inner clip area
			//ctx.Rectangle(drawBounds.X, drawBounds.Y, drawBounds.Width, drawBounds.Height);
			//ctx.Clip();
			
			if (curve.Count > 0)
			{
				SetColor(ctx, colorLine);
				
				int numSteps = (int)drawBounds.Width;

				var p = Evaluate(0, drawBounds);
				ctx.MoveTo(p.X, p.Y);
				ctx.LineWidth = 1.0;
				SetDash(ctx, true);

				var startFraction = curve.Front.Value.X;
				var endFraction = curve.End.Value.X;
				int part = 0;
				
				for (int i = 0; i < numSteps; i++)
				{
					var fraction = (float)i / numSteps;
					
					if (fraction >= startFraction && part == 0)
					{
						p = ToScreen(curve.Front.Value, drawBounds);
						ctx.LineTo(p.X, p.Y);
						ctx.Stroke();
						SetDash(ctx, false);												
						ctx.MoveTo(p.X, p.Y);

						part++;
					}

					if (fraction >= endFraction && part == 1)
					{
						p = ToScreen(curve.End.Value, drawBounds);
						ctx.LineTo(p.X, p.Y);
						ctx.Stroke();
						SetDash(ctx, true);
						ctx.MoveTo(p.X, p.Y);

						part++;
					}
					
					p = Evaluate(fraction, drawBounds);
					ctx.LineTo(p.X, p.Y);
				}

				ctx.Stroke();
				SetDash(ctx, false);

				foreach (var entry in curve)
				{
					var b = ToScreen(entry.Value, drawBounds);
					float size = 4;

					SetColor(ctx, entry == currentEntry ? (movingEntry ? colorSelectedEntry : colorHoveredEntry) : colorLine);
					ctx.Rectangle(b.X - size / 2, b.Y - size / 2, size, size);
					ctx.Fill();
				}
			}

			ctx.Restore();
		}

		private void SetColor(Cairo.Context ctx, MG.Framework.Numerics.Color color)
		{
			ctx.SetSourceRGBA(color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0);
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

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			using (Cairo.Context c = Gdk.CairoHelper.Create(evnt.Window))
			{
				Draw(evnt.Window, c, evnt.Area, StateType.Selected);
			}
			return true;
		}
		
		private void DrawingAreaOnExposeEvent(object o, ExposeEventArgs args)
		{
			using (Cairo.Context c = Gdk.CairoHelper.Create(args.Event.Window))
			{
				Draw(args.Event.Window, c, drawingArea.Allocation, StateType.Selected);
			}
		}
		
		private RectangleF GraphAreaFromBounds(RectangleF bounds)
		{
			int padding = 3;
			int paddingRight = 4;
			int paddingBottom = 2;
			return new RectangleF(bounds.X + padding, bounds.Y + padding, bounds.Width - padding * 2 - paddingRight, bounds.Height - padding * 2 - paddingBottom);
		}

		private RectangleF GraphArea
		{
			get
			{
				var area = Allocation;
				area.X = 0;
				area.Y = 0;
				return GraphAreaFromBounds(new RectangleF(area.X, area.Y, area.Width, area.Height));
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

		protected override bool OnMotionNotifyEvent(EventMotion evnt)
		{
			if (curve == null) return true;

			var mousePos = new Vector2((float)evnt.X, (float)evnt.Y);

			if (movingEntry)
			{
				UpdateEntryPosition(mousePos);
			}
			else
			{
				var oldEntry = currentEntry;
				currentEntry = GetEntryAt(mousePos);
				if (currentEntry != oldEntry)
				{
					QueueDraw();
				}
			}

			return true;
		}

		protected override bool OnButtonPressEvent(EventButton evnt)
		{
			var area = GraphArea;
			var mousePos = new Vector2((float)evnt.X, (float)evnt.Y);

			if (!movingEntry)
			{
				currentEntry = GetEntryAt(mousePos);
				if (currentEntry != null)
				{
					if (!movingEntry)
					{
						movingEntry = true;
						QueueDraw();
					}
				}
				else
				{
					var p = FromScreen(mousePos, area);
					var closestP = Evaluate(p.X, area);
					
					if (curve.Count == 0 || Math.Abs(closestP.Y - mousePos.Y) < 6)
					{
						currentEntry = CreateCurveEntry(p);
						movingEntry = true;
					}
				}
			}
			
			if (movingEntry)
			{
				UpdateEntryPosition(mousePos);
			}
			
			return true;
		}
		
		protected override bool OnButtonReleaseEvent(EventButton evnt)
		{
			if (movingEntry)
			{
				movingEntry = false;
				QueueDraw();
			}
			
			return true;
		}

		internal void KeyPress(EventKey evnt)
		{
			// Hack: can't get normal keypress event for some reason, so hook it up from the parent

			if (currentEntry != null && (evnt.Key == Key.Delete || evnt.Key == Key.BackSpace))
			{
				RemoveCurveEntry(currentEntry);
			}
		}

		private void UpdateEntryPosition(Vector2 position)
		{
			if (currentEntry == null) return;

			var area = GraphArea;
			var p = ToScreen(currentEntry.Value, area);
			if ((p - position).Length() < 0.1f) return;

			p = FromScreen(position, area);

			currentEntry = ReplaceCurveEntry(currentEntry, p);
		}
		
		private CurveEntry GetEntryAt(Vector2 screenPos)
		{
			if (curve == null) return null;
			var area = GraphArea;

			foreach (var entry in curve)
			{
				var p = ToScreen(entry.Value, area);
				var length = (p - screenPos).Length();

				if (length < 8)
				{
					return entry;
				}
			}
			
			return null;
		}

		private void OnCurveChange()
		{
			QueueDraw();
			Changed.Invoke();
		}

		private CurveEntry CreateCurveEntry(Vector2 position)
		{
			var entry = new CurveEntry(new Vector2(MathTools.ClampNormal(position.X), MathTools.ClampNormal(position.Y)));
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private CurveEntry ReplaceCurveEntry(CurveEntry oldEntry, Vector2 position)
		{
			var entry = new CurveEntry(new Vector2(MathTools.ClampNormal(position.X), MathTools.ClampNormal(position.Y)));
			curve.Remove(oldEntry);
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private void RemoveCurveEntry(CurveEntry entry)
		{
			curve.Remove(entry);
			OnCurveChange();
		}
	}
}

