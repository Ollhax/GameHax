using System;
using System.Collections.Generic;

using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;
using Curve = MG.Framework.Numerics.Curve;
using Key = Gdk.Key;
using Rectangle = Gdk.Rectangle;

namespace MG.EditorCommon.HaxWidgets
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

		public float MinValueX = 0;
		public float MaxValueX = 1;
		public float MinValueY = 0;
		public float MaxValueY = 1;

		private float RangeX { get { return MaxValueX - MinValueX; } }
		private float RangeY { get { return MaxValueY - MinValueY; } }
		private bool HasZeroLine { get { return MinValueY < 0 && MaxValueY > 0; } }

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

			var colorSelectedBackground = WidgetTools.ColorSelectedBackground;
			var colorBorder = WidgetTools.ColorBorder;
			var colorLine = WidgetTools.ColorLine;
			var colorHoveredEntry = WidgetTools.ColorHoveredEntry;
			var colorSelectedEntry = WidgetTools.ColorSelectedEntry;

			// Background
			if (state == StateType.Selected)
			{
				WidgetTools.SetColor(ctx, colorSelectedBackground);
				ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
				ctx.Fill();
			}
			
			// Border
			WidgetTools.SetColor(ctx, colorBorder);
			ctx.LineWidth = 1.0;
			ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
			ctx.Stroke();

			// Zero line
			if (HasZeroLine)
			{
				var height = ToScreen(Vector2.Zero, drawBounds);
				WidgetTools.SetColor(ctx, colorBorder);
				ctx.MoveTo(outerBounds.X, height.Y);
				ctx.LineTo(outerBounds.X + outerBounds.Width, height.Y);
				ctx.LineWidth = 1.0;
				WidgetTools.SetDash(ctx, true);
				ctx.Stroke();
			}
			
			//// Inner clip area
			//ctx.Rectangle(drawBounds.X, drawBounds.Y, drawBounds.Width, drawBounds.Height);
			//ctx.Clip();
			
			if (curve.Count > 0)
			{
				WidgetTools.SetColor(ctx, colorLine);
				
				int numSteps = (int)drawBounds.Width;

				var p = Evaluate(0, drawBounds);
				ctx.MoveTo(p.X, p.Y);
				ctx.LineWidth = 1.2;
				WidgetTools.SetDash(ctx, true);

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
						WidgetTools.SetDash(ctx, false);												
						ctx.MoveTo(p.X, p.Y);

						part++;
					}

					if (fraction >= endFraction && part == 1)
					{
						p = ToScreen(curve.End.Value, drawBounds);
						ctx.LineTo(p.X, p.Y);
						ctx.Stroke();
						WidgetTools.SetDash(ctx, true);
						ctx.MoveTo(p.X, p.Y);

						part++;
					}
					
					p = Evaluate(fraction, drawBounds);
					ctx.LineTo(p.X, p.Y);
				}

				ctx.Stroke();
				WidgetTools.SetDash(ctx, false);

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
						WidgetTools.SetColor(ctx, outerColor);

						if (entry.Type == CurveEntry.EntryType.Bezier)
						{
							ctx.MoveTo(b.X, b.Y);
							p = ToScreen(entry.LeftHandle, drawBounds);
							ctx.LineTo(p.X, p.Y);
							ctx.Stroke();

							WidgetTools.SetColor(ctx, hoveredHandle == Handle.Left ? colorHoveredEntry : outerColor);
							ctx.Arc(p.X, p.Y, 2, 0, 2 * Math.PI);
							ctx.Fill();

							WidgetTools.SetColor(ctx, outerColor);
							ctx.MoveTo(b.X, b.Y);
							p = ToScreen(entry.RightHandle, drawBounds);
							ctx.LineTo(p.X, p.Y);
							ctx.Stroke();

							WidgetTools.SetColor(ctx, hoveredHandle == Handle.Right ? colorHoveredEntry : outerColor);
							ctx.Arc(p.X, p.Y, 2, 0, 2 * Math.PI);
							ctx.Fill();
						}
					}

					if (outerColor.A != 0)
					{
						WidgetTools.SetColor(ctx, outerColor);
						ctx.Rectangle(b.X - outerSize / 2, b.Y - outerSize / 2, outerSize, outerSize);
						ctx.LineWidth = 1.0;
						ctx.Stroke();
					}
					
					WidgetTools.SetColor(ctx, color);
					ctx.Rectangle(b.X - size / 2, b.Y - size / 2, size, size);
					ctx.Fill();
				}
			}

			ctx.Restore();
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
			return new Vector2(
				area.X + ((value.X - MinValueX) / RangeX) * area.Width,
				area.Y + area.Height - ((value.Y - MinValueY) / RangeY) * area.Height);
		}

		private Vector2 FromScreen(Vector2 screenPoint, RectangleF area)
		{
			return new Vector2(
				MinValueX + ((screenPoint.X - area.X) / area.Width) * RangeX,
				MinValueY + RangeY - ((screenPoint.Y - area.Y) / area.Height) * RangeY);
		}
		
		private Vector2 ClampValue(Vector2 value)
		{
			if (value.X < MinValueX) value.X = MinValueX;
			if (value.X > MaxValueX) value.X = MaxValueX;
			if (value.Y < MinValueY) value.Y = MinValueY;
			if (value.Y > MaxValueY) value.Y = MaxValueY;
			return value;
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
						//GdkWindow.Cursor = hoveredEntry != null ? Resources.HandCursor : null;
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
				return LeftMousePress(mousePos, evnt.Type == EventType.TwoButtonPress);
			}
			
			if (evnt.Button == 3)
			{
				return RightMousePress(mousePos);
			}

			return true;
		}

		private bool LeftMousePress(Vector2 position, bool doubleClick)
		{
			var area = GraphArea;

			//if (doubleClick && selectedEntry != null)
			//{
			//    movingEntry = false;
			//    selectedEntry = SwitchCurveEntry(
			//        selectedEntry,
			//        selectedEntry.Type == CurveEntry.EntryType.Linear ? CurveEntry.EntryType.Bezier : CurveEntry.EntryType.Linear);
			//}

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

						if (curve.Count == 0 || (Math.Abs(closestP.Y - position.Y) < 12 || doubleClick))
						{
							selectedEntry = CreateCurveEntry(p);
							movingEntry = true;
						}
					}
				}
			}

			//if (movingEntry)
			//{
			//    UpdateEntryPosition(position);
			//}

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

				AddMenuEntry(m, "Linear", Resources.IconCurveLinear, delegate { selectedEntry = SwitchCurveEntry(currentEntry, CurveEntry.EntryType.Linear); });
				AddMenuEntry(m, "Bezier", Resources.IconCurveBezier, delegate { selectedEntry = SwitchCurveEntry(currentEntry, CurveEntry.EntryType.Bezier); });
				m.Add(new SeparatorMenuItem());
				AddMenuEntry(m, "Delete", Resources.IconCurveDelete, delegate { RemoveCurveEntry(currentEntry); });
			}

			AddMenuEntry(m, "Clear All", Resources.IconCancel, delegate { ClearCurve(); });

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
			
			if (HasZeroLine)
			{
				if (Math.Abs(p.Y) < 0.03f * RangeY)
				{
					p.Y = 0;
				}
			}

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
			var newPos = ClampValue(position);
			var entry = new CurveEntry(newPos);
			
			curve.Add(entry);
			OnCurveChange();
			return entry;
		}

		private CurveEntry ReplaceCurveEntry(CurveEntry oldEntry, Vector2 position)
		{
			var newPos = ClampValue(position);
			
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

