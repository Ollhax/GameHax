using Cairo;

using Gtk;
using Gdk;

using MG.Framework.Numerics;

using Action = System.Action;
using Color = MG.Framework.Numerics.Color;
using Gradient = MG.Framework.Numerics.Gradient;
using Key = Gdk.Key;
using Rectangle = Gdk.Rectangle;

namespace MG.EditorCommon.HaxGradient
{
	[System.ComponentModel.ToolboxItem(true)]
	public class HaxGradient : DrawingArea
	{
		private Gradient gradient;
		private GradientEntry hoveredEntry;
		private GradientEntry selectedEntry;
		private bool movingEntry;
		
		private void ClearState()
		{
			hoveredEntry = null;
			selectedEntry = null;
			movingEntry = false;
		}
		
		public event Action Changed = delegate { };

		public Gradient Gradient
		{
			get { return gradient; }

			set
			{
				if (gradient == value) return;
				gradient = value;
				
				QueueDraw();
			}
		}

		public HaxGradient()
		{
			ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255)); // Disable graying on selection
			AddEvents((int)(EventMask.AllEventsMask));
		}

		private void DrawBackground(Context ctx, RectangleF area)
		{
			var colorWhite = Color.White;
			var colorBlack = new Color(230, 230, 230, 255);
			int size = 6;
			int count = 0;

			for (int x = 0; x < area.Width / size; x++)
			{
				for (int y = 0; y < area.Height / size; y++)
				{
					bool even = (x % 2 == 0) ^ (y % 2 == 0);
					
					SetColor(ctx, even ? colorWhite : colorBlack);
					ctx.Rectangle(area.X + x * size, area.Y + y * size, size, size);
					ctx.Fill();
				}	
			}
		}

		public void Draw(Drawable window, Context ctx, Rectangle bounds, StateType state)
		{
			if (gradient == null) return;

			int outerBoundsThickness = 4;

			var area = AreaFromBounds(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
			var outerBounds = area.Inflated(outerBoundsThickness);
			var colorBorder = new Color(204, 204, 204, 255);
			var colorSelectedBackground = new Color(154, 154, 154, 255);
			var colorEntry = Color.Black;
			
			ctx.Save();

			ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
			ctx.Clip();
			
			// Border
			SetColor(ctx, state == StateType.Selected ? colorSelectedBackground : colorBorder);
			//SetColor(ctx, colorBorder);
			ctx.LineWidth = 1.0;
			ctx.Antialias = Antialias.Default;
			ctx.Rectangle(outerBounds.X, outerBounds.Y, outerBounds.Width, outerBounds.Height);
			ctx.Stroke();

			// Clip inner area
			ctx.Rectangle(outerBounds.X + 1, outerBounds.Y + 1, outerBounds.Width - 2, outerBounds.Height - 2);
			ctx.Clip();

			// Background
			DrawBackground(ctx, outerBounds);

			// Gradient
			if (gradient.Count > 0 && area.Width > 0)
			{
			    int numSteps = (int)outerBounds.Width;
				
			    // Could be done much cheaper with cairo gradients, but this way we'll show the actual values
			    for (int i = 0; numSteps > 0 && i < numSteps; i++)
			    {
					var fraction = (i - outerBoundsThickness) / area.Width;
					fraction = MathTools.ClampNormal(fraction);
					
			        SetColor(ctx, gradient.Evaluate(fraction));

					float x = outerBounds.Position.X + i;
					float y = outerBounds.Position.Y;
					ctx.MoveTo(x, y);
					ctx.LineTo(x, y + outerBounds.Height);
					ctx.Antialias = Antialias.None;
					ctx.Stroke();
			    }
			}

			// Entries
			foreach (var entry in gradient)
			{
				var slice = GetEntryArea(entry.Position, area);

				ctx.Antialias = Antialias.None;

				ctx.Rectangle(slice.X, slice.Y, slice.Width, slice.Height);
				SetColor(ctx, entry.Color);
				ctx.Fill();

				Color edgeColor = entry == hoveredEntry ? GetComplementary1(entry.Color) : GetComplementary2(entry.Color);

				ctx.Rectangle(slice.X, slice.Y, slice.Width, slice.Height);
				ctx.LineWidth = 1;
				SetColor(ctx, edgeColor);
				ctx.Stroke();

				if (entry == selectedEntry)
				{
					ctx.Rectangle(slice.X + 1, slice.Y + 1, slice.Width - 2, slice.Height - 2);
					ctx.LineWidth = 1;
					SetColor(ctx, edgeColor);
					ctx.Stroke();
				}
			}
			
			ctx.Restore();
		}

		private Color GetComplementary1(Color color)
		{
			var c = new Color(255 - color.R, 255 - color.G, 255 - color.B, 255);
			int darkest = 50;
			int brightest = 255 + 255 + 255 - darkest;

			if (c.R + c.G + c.B < darkest)
			{
				return Color.Red;
			}

			if (c.R + c.G + c.B > brightest)
			{
				return Color.Red;
			}

			return c;
		}

		private Color GetComplementary2(Color color)
		{
			var c = new Color(255 - color.B, 255 - color.G, 255 - color.R, 255);
			int darkest = 50;
			int brightest = 255 + 255 + 255 - darkest;

			if (c.R + c.G + c.B < darkest)
			{
				return Color.Red;
			}

			if (c.R + c.G + c.B > brightest)
			{
				return Color.Red;
			}

			return c;
		}

		private RectangleF GetEntryArea(float fraction, RectangleF area)
		{
			float x = area.Position.X + area.Width * fraction;
			float y = area.Position.Y;
			return new RectangleF(x, y, 1, area.Height + 1).Inflated(2);
		}

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			using (Context c = CairoHelper.Create(evnt.Window))
			{
				var area = LocalAllocation;
				Draw(evnt.Window, c, area, StateType.Selected);
			}
			return true;
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

		private RectangleF AreaFromBounds(RectangleF bounds)
		{
			const int padding = 4;
			const int paddingRight = 4;
			const int paddingBottom = 2;
			return new RectangleF(bounds.X + padding, bounds.Y + padding, bounds.Width - padding * 2 - paddingRight, bounds.Height - padding * 2 - paddingBottom);
		}

		private RectangleF Area
		{
			get
			{
				var area = LocalAllocation;
				return AreaFromBounds(new RectangleF(area.X, area.Y, area.Width, area.Height));
			}
		}

		private float FromScreen(float value, RectangleF area)
		{
			return (value - area.X) / area.Width;
		}
		
		private void SetColor(Context ctx, Color color)
		{
			ctx.SetSourceRGBA(color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0);
		}
		
		protected override bool OnMotionNotifyEvent(EventMotion evnt)
		{
			if (gradient == null) return true;

			var area = Area;
			var mousePos = new Vector2((float)evnt.X, (float)evnt.Y);
			float value = FromScreen(mousePos.X, area);

			if (movingEntry)
			{
				UpdateEntryPosition(value);
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
			var area = Area;
			float value = FromScreen(position.X, area);

			if (doubleClick && selectedEntry != null)
			{
				PickColor();
				movingEntry = false;
				return true;
			}

			if (!movingEntry)
			{
				selectedEntry = GetEntryAt(position);
				QueueDraw();

				if (selectedEntry != null)
				{
					movingEntry = true;
				}
				else
				{
					selectedEntry = CreateEntry(value);
					movingEntry = true;
				}
			}

			//if (movingEntry)
			//{
			//    UpdateEntryPosition(value);
			//}

			return true;
		}

		private bool RightMousePress(Vector2 position)
		{
			var m = new Menu();

			var entry = GetEntryAt(position);
			if (entry != null)
			{
				selectedEntry = entry;
				QueueDraw();
			}

			if (selectedEntry != null)
			{
				var currentEntry = selectedEntry;

				AddMenuEntry(m, "Choose Color...", Resources.IconColorWheel, delegate { PickColor(); });
				m.Add(new SeparatorMenuItem());
				AddMenuEntry(m, "Delete", Resources.IconDelete, delegate { RemoveEntry(currentEntry); });
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
			
			return true;
		}

		internal void KeyPress(EventKey evnt)
		{
			// Hack: can't get normal keypress event for some reason, so hook it up from the parent

			if (selectedEntry != null && (evnt.Key == Key.Delete || evnt.Key == Key.BackSpace))
			{
				RemoveEntry(selectedEntry);
			}
		}

		private byte FromGdkColor(ushort alpha)
		{
			return (byte)(alpha / 257);
		}

		private ushort ToGdkColor(byte alpha)
		{
			return (ushort)(alpha * 257);
		}

		private void PickColor()
		{
			if (selectedEntry == null) return;
			
			var dlg = new ColorSelectionDialog("Pick a color");
			dlg.ColorSelection.HasOpacityControl = true;
			dlg.ColorSelection.CurrentColor = new Gdk.Color(selectedEntry.Color.R, selectedEntry.Color.G, selectedEntry.Color.B);
			dlg.ColorSelection.CurrentAlpha = ToGdkColor(selectedEntry.Color.A);
			dlg.Modal = true;

			dlg.Response += delegate(object o, ResponseArgs args)
			{
				if (args.ResponseId == ResponseType.Ok)
				{
					var newColor = new Color(
					FromGdkColor(dlg.ColorSelection.CurrentColor.Red),
					FromGdkColor(dlg.ColorSelection.CurrentColor.Green),
					FromGdkColor(dlg.ColorSelection.CurrentColor.Blue),
					FromGdkColor(dlg.ColorSelection.CurrentAlpha));

					selectedEntry = ReplaceEntry(selectedEntry, newColor);
				}
			};

			dlg.Run();
			dlg.Destroy();
		}
		
		private void UpdateEntryPosition(float position)
		{
			if (selectedEntry == null) return;
			position = MathTools.ClampNormal(position);
			
			selectedEntry = ReplaceEntry(selectedEntry, position);
		}

		private GradientEntry GetEntryAt(Vector2 screenPos)
		{
			if (gradient == null) return null;
			var area = Area;

			foreach (var entry in gradient)
			{
				if (GetEntryArea(entry.Position, area).Inflated(5).Contains(screenPos))
					return entry;
			}

			return null;
		}

		private void OnChange()
		{
			QueueDraw();
			Changed.Invoke();
		}

		private GradientEntry CreateEntry(float position)
		{
			position = MathTools.ClampNormal(position);
			var entry = new GradientEntry(position, gradient.Evaluate(position));

			gradient.Add(entry);
			OnChange();
			return entry;
		}

		private GradientEntry ReplaceEntry(GradientEntry oldEntry, float position)
		{
			position = MathTools.ClampNormal(position);
			
			var entry = new GradientEntry(position, oldEntry.Color);
			gradient.Remove(oldEntry);
			gradient.Add(entry);
			OnChange();
			return entry;
		}

		private GradientEntry ReplaceEntry(GradientEntry oldEntry, Color color)
		{
			var entry = new GradientEntry(oldEntry.Position, color);
			gradient.Remove(oldEntry);
			gradient.Add(entry);
			OnChange();
			return entry;
		}

		private void RemoveEntry(GradientEntry entry)
		{
			gradient.Remove(entry);
			ClearState();
			OnChange();
		}

		private void ClearCurve()
		{
			gradient.Clear();
			ClearState();
			OnChange();
		}
	}
}

