
using MG.Framework.Numerics;

namespace MG.EditorCommon.HaxWidgets
{
	static class WidgetTools
	{
		public static Color ColorSelectedBackground = new Color(240, 240, 255, 255);
		public static Color ColorBorder = new Color(204, 204, 204, 255);
		public static Color ColorLine = new Color(255, 0, 0, 255);
		public static Color ColorHoveredEntry = new Color(0, 240, 0, 255);
		public static Color ColorSelectedEntry = new Color(0, 0, 0, 255);

		public static void SetColor(Cairo.Context ctx, Color color)
		{
			ctx.SetSourceRGBA(color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0);
		}

		public static void SetDash(Cairo.Context ctx, bool enabled)
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
	}
}
