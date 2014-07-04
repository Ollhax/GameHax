using Cairo;

using Gdk;

using Gtk;

using MG.Framework.Numerics;

using Color = Gdk.Color;
using Rectangle = Gdk.Rectangle;

namespace MG.ParticleEditorWindow
{
	class ColorMenuItem : RadioMenuItem
	{
		public MG.Framework.Numerics.Color Color;

		public ColorMenuItem(string label)
			: base(label)
		{

		}

		private void DrawBackground(Context ctx, RectangleF area)
		{
			var colorWhite = Framework.Numerics.Color.White;
			var colorBlack = new Framework.Numerics.Color(230, 230, 230, 255);
			int size = 6;

			for (int x = 0; x < area.Width / size; x++)
			{
				for (int y = 0; y < area.Height / size; y++)
				{
					bool even = (x % 2 == 0) ^ (y % 2 == 0);
					var c = even ? colorWhite : colorBlack;
					ctx.SetSourceRGB(c.R / 255.0, c.G / 255.0, c.B / 255.0);
					ctx.Rectangle(area.X + x * size, area.Y + y * size, size, size);
					ctx.Fill();
				}
			}
		}

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			base.OnExposeEvent(evnt);

			using (Context c = CairoHelper.Create(evnt.Window))
			{
				var bounds = Allocation;
				int size = (int)(bounds.Height * 0.66f);
				var area = new RectangleF(bounds.X + (int)(bounds.Width * 0.67f), bounds.Y + (bounds.Height - size) / 2, size, size);

				if (Color.A != 255)
				{
					c.Rectangle(area.X, area.Y, area.Width, area.Height);
					c.Clip();
					DrawBackground(c, area);
					c.ResetClip();
				}
				
				CairoHelper.SetSourceColor(c, Style.Dark(StateType.Normal));
				c.LineWidth = 1;
				c.Rectangle(area.X, area.Y, area.Width, area.Height);
				c.Stroke();

				c.SetSourceRGBA(Color.R / 255.0, Color.G / 255.0, Color.B / 255.0, Color.A / 255.0f);
				c.Rectangle(area.X+1, area.Y+1, area.Width-2, area.Height-2);
				c.Fill();
			}
			return true;
		}
	}
}
