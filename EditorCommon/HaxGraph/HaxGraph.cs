using System;

using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;
using Curve = MG.Framework.Numerics.Curve;
using Key = Gdk.Key;
using Rectangle = Gdk.Rectangle;

namespace MG.EditorCommon.HaxGraph
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGraph : DrawingArea
	{
		enum Handle
		{
			None,
			Left,
			Right,
		}

		private Curve curve;
		private CurveEntry hoveredEntry;
		private CurveEntry selectedEntry;
		private bool movingEntry;
		private Handle hoveredHandle = Handle.None;
		private Handle movingHandle = Handle.None;

		private void ClearState()
		{
			hoveredEntry = null;
			selectedEntry = null;
			movingEntry = false;
			hoveredHandle = Handle.None;
			movingHandle = Handle.None;
		}
		
		public event Action Changed = delegate { };

		private Gdk.Pixbuf iconCurveDelete;
		private Gdk.Pixbuf iconCurveLinear;
		private Gdk.Pixbuf iconCurveBezier;
		private Gdk.Pixbuf iconCurveClear;
		
		public Curve Curve
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
			
			iconCurveDelete = Gdk.Pixbuf.LoadFromResource("chart_curve_delete.png");
			iconCurveBezier = Gdk.Pixbuf.LoadFromResource("chart_curve.png");
			iconCurveLinear = Gdk.Pixbuf.LoadFromResource("chart_line.png");
			iconCurveClear = Gdk.Pixbuf.LoadFromResource("cancel.png");
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
			var colorSelectedEntry = new MG.Framework.Numerics.Color(0, 0, 0, 255);

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
				ctx.LineWidth = 1.2;
				SetDash(ctx, true);

				var startFraction = curve.Front.Value.X;
				var endFraction = curve.End.Value.X;
				int part = 0;

				for (int i = 0; numSteps > 0 && i <= numSteps; i++)
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
					float outerSize = size + 1;

					var color = colorLine;
					var outerColor = MG.Framework.Numerics.Color.Transparent;
					if (entry == hoveredEntry)
					{
						color = colorHoveredEntry;
					}
					
					if (entry == selectedEntry)
					{
						color = colorHoveredEntry;
						outerColor = colorSelectedEntry;
						
						ctx.LineWidth = 1.0;
						SetColor(ctx, outerColor);

						if (entry.Type == CurveEntry.EntryType.Bezier)
						{
							ctx.MoveTo(b.X, b.Y);
							p = ToScreen(entry.LeftHandle, drawBounds);
							ctx.LineTo(p.X, p.Y);
							ctx.Stroke();

							SetColor(ctx, hoveredHandle == Handle.Left ? colorHoveredEntry : outerColor);
							ctx.Arc(p.X, p.Y, 2, 0, 2 * Math.PI);
							ctx.Fill();

							SetColor(ctx, outerColor);
							ctx.MoveTo(b.X, b.Y);
							p = ToScreen(entry.RightHandle, drawBounds);
							ctx.LineTo(p.X, p.Y);
							ctx.Stroke();

							SetColor(ctx, hoveredHandle == Handle.Right ? colorHoveredEntry : outerColor);
							ctx.Arc(p.X, p.Y, 2, 0, 2 * Math.PI);
							ctx.Fill();
						}
					}

					if (outerColor.A != 0)
					{
						SetColor(ctx, outerColor);
						ctx.Rectangle(b.X - outerSize / 2, b.Y - outerSize / 2, outerSize, outerSize);
						ctx.LineWidth = 1.0;
						ctx.Stroke();
					}
					
					SetColor(ctx, color);
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
				var area = LocalAllocation; //evnt.Area
				Draw(evnt.Window, c, area, StateType.Selected);
			}
			return true;
		}
		
		private RectangleF GraphAreaFromBounds(RectangleF bounds)
		{
			int padding = 3;
			int paddingRight = 4;
			int paddingBottom = 2;
			return new RectangleF(bounds.X + padding, bounds.Y + padding, bounds.Width - padding * 2 - paddingRight, bounds.Height - padding * 2 - paddingBottom);
		}

		private Rectangle LocalAllocation
		{
			get
			{
				var area = Allocation;
				area.X = 0;
				area.Y = 0;
				return area;
			}
		}

		private RectangleF GraphArea
		{
			get
			{
				var area = LocalAllocation;
				return GraphAreaFromBounds(new RectangleF(area.X, area.Y, area.Width, area.Height));
			}
		}
		
		private Vector2 Evaluate(float x, RectangleF area)
		{
			var y = curve.Evaluate(x);
			return ToScreen(new Vector2(x, y), area);
		}

		private Handle GetHandle(Vector2 screenPoint, RectangleF area)
		{
			if (selectedEntry == null) return Handle.None;
			if (selectedEntry.Type != CurveEntry.EntryType.Bezier) return Handle.None;

			const float distance = 4;

			var p = ToScreen(selectedEntry.LeftHandle, area);
			if ((screenPoint - p).Length() < distance) return Handle.Left;
			
			p = ToScreen(selectedEntry.RightHandle, area);
			if ((screenPoint - p).Length() < distance) return Handle.Right;

			return Handle.None;
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

			if (movingHandle != Handle.None)
			{
				UpdateHandlePosition(mousePos);
			}
			if (movingEntry)
			{
				UpdateEntryPosition(mousePos);
			}
			else
			{
				var oldHandle = hoveredHandle;
				hoveredHandle = GetHandle(mousePos, GraphArea);
				if (hoveredHandle != oldHandle)
				{
					QueueDraw();
				}
				else
				{
					var oldEntry = hoveredEntry;
					hoveredEntry = GetEntryAt(mousePos);
					if (hoveredEntry != oldEntry)
					{
						QueueDraw();
					}
				}
			}

			return true;
		}

		protected override bool OnButtonPressEvent(EventButton evnt)
		{
			var mousePos = new Vector2((float)evnt.X, (float)evnt.Y);
			if (evnt.Button == 1)
			{
				return LeftMousePress(mousePos);
			}
			
			if (evnt.Button == 3)
			{
				return RightMousePress(mousePos);
			}

			return true;
		}
		
		private bool LeftMousePress(Vector2 position)
		{
			var area = GraphArea;

			if (!movingEntry)
			{
				if (movingHandle == Handle.None)
				{
					movingHandle = GetHandle(position, area);
				}

				if (movingHandle == Handle.None)
				{
					selectedEntry = GetEntryAt(position);
					QueueDraw();

					if (selectedEntry != null)
					{
						movingHandle = GetHandle(position, area);

						if (movingHandle == Handle.None && !movingEntry)
						{
							movingEntry = true;
						}
					}
					else
					{
						var p = FromScreen(position, area);
						var closestP = Evaluate(p.X, area);

						if (curve.Count == 0 || Math.Abs(closestP.Y - position.Y) < 12)
						{
							selectedEntry = CreateCurveEntry(p);
							movingEntry = true;
						}
					}
				}
			}

			if (movingEntry)
			{
				UpdateEntryPosition(position);
			}

			return true;
		}

		private bool RightMousePress(Vector2 position)
		{
			selectedEntry = GetEntryAt(position);
			QueueDraw();

			var m = new Menu();

			if (selectedEntry != null)
			{
				var currentEntry = selectedEntry;

				AddMenuEntry(m, "Linear", iconCurveLinear, delegate { selectedEntry = SwitchCurveEntry(currentEntry, CurveEntry.EntryType.Linear); });
				AddMenuEntry(m, "Bezier", iconCurveBezier, delegate { selectedEntry = SwitchCurveEntry(currentEntry, CurveEntry.EntryType.Bezier); });
				m.Add(new SeparatorMenuItem());
				AddMenuEntry(m, "Delete", iconCurveDelete, delegate { RemoveCurveEntry(currentEntry); });
			}

			AddMenuEntry(m, "Clear All", iconCurveClear, delegate { ClearCurve(); });

			m.ShowAll();
			m.Popup();

			return true;
		}

		private void AddMenuEntry(Menu menu, string text, Pixbuf icon, Action action)
		{
			var item = new ImageMenuItem(text);
			item.Image = new Gtk.Image(icon);
			item.ButtonPressEvent += delegate { action(); };
			menu.Add(item);
		}

		protected override bool OnButtonReleaseEvent(EventButton evnt)
		{
			if (movingEntry)
			{
				movingEntry = false;
				QueueDraw();
			}

			if (movingHandle != Handle.None)
			{
				movingHandle = Handle.None;
				QueueDraw();
			}
			
			return true;
		}

		internal void KeyPress(EventKey evnt)
		{
			// Hack: can't get normal keypress event for some reason, so hook it up from the parent

			if (selectedEntry != null && (evnt.Key == Key.Delete || evnt.Key == Key.BackSpace))
			{
				RemoveCurveEntry(selectedEntry);
			}

			if (selectedEntry != null && (evnt.Key == Key.b))
			{
				selectedEntry = SwitchCurveEntry(selectedEntry, CurveEntry.EntryType.Bezier);
			}
			if (selectedEntry != null && (evnt.Key == Key.n))
			{
				selectedEntry = SwitchCurveEntry(selectedEntry, CurveEntry.EntryType.Linear);
			}
		}

		private void UpdateHandlePosition(Vector2 position)
		{
			if (selectedEntry == null) return;
			if (movingHandle == Handle.None) return;

			var area = GraphArea;
			var leftHandle = selectedEntry.LeftHandle;
			var rightHandle = selectedEntry.RightHandle;
			var pos = FromScreen(position, area);

			if (movingHandle == Handle.Left)
			{
				leftHandle = pos;
			}
			else
			{
				rightHandle = pos;
			}
			
			selectedEntry = ReplaceCurveHandles(selectedEntry, leftHandle, rightHandle);
		}

		private void UpdateEntryPosition(Vector2 position)
		{
			if (selectedEntry == null) return;

			var area = GraphArea;
			var p = ToScreen(selectedEntry.Value, area);
			if ((p - position).Length() < 0.1f) return;

			p = FromScreen(position, area);

			selectedEntry = ReplaceCurveEntry(selectedEntry, p);
		}
		
		private CurveEntry GetEntryAt(Vector2 screenPos)
		{
			if (curve == null) return null;
			var area = GraphArea;

			foreach (var entry in curve)
			{
				var p = ToScreen(entry.Value, area);
				var length = (p - screenPos).Length();

				if (length < 12)
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
			var newPos = new Vector2(MathTools.ClampNormal(position.X), MathTools.ClampNormal(position.Y));
			var entry = new CurveEntry(newPos);
			
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private CurveEntry ReplaceCurveEntry(CurveEntry oldEntry, Vector2 position)
		{
			var newPos = new Vector2(MathTools.ClampNormal(position.X), MathTools.ClampNormal(position.Y));
			
			var diff = newPos - oldEntry.Value;
			var entry = new CurveEntry(oldEntry.Type, newPos, diff + oldEntry.LeftHandle, diff + oldEntry.RightHandle);
			curve.Remove(oldEntry);
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private CurveEntry ReplaceCurveHandles(CurveEntry oldEntry, Vector2 leftHandle, Vector2 rightHandle)
		{
			var entry = new CurveEntry(oldEntry.Type, oldEntry.Value, leftHandle, rightHandle);
			curve.Remove(oldEntry);
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private void RemoveCurveEntry(CurveEntry entry)
		{
			curve.Remove(entry);
			ClearState();
			OnCurveChange();
		}

		private void ClearCurve()
		{
			curve.Clear();
			ClearState();
			OnCurveChange();
		}

		private CurveEntry SwitchCurveEntry(CurveEntry oldEntry, CurveEntry.EntryType newType)
		{
			CurveEntry entry = null;
			const float bezierOffset = 0.1f;

			switch (newType)
			{
				case CurveEntry.EntryType.Linear: { entry = new CurveEntry(newType, oldEntry.Value, oldEntry.LeftHandle, oldEntry.RightHandle); } break;
				case CurveEntry.EntryType.Bezier: { entry = new CurveEntry(newType, oldEntry.Value, oldEntry.Value - new Vector2(bezierOffset, 0.0f), oldEntry.Value + new Vector2(bezierOffset, 0.0f)); } break;
			}

			if (entry == null)
				throw new NotImplementedException("curve entry type");

			curve.Remove(oldEntry);
			curve.Add(entry);
			OnCurveChange();
			
			return entry;
		}
	}
}

