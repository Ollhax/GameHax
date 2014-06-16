using Gdk;

namespace MG.EditorCommon
{
	static class Resources
	{
		public static readonly Gdk.Pixbuf IconCurveDelete;
		public static readonly Gdk.Pixbuf IconCurveLinear;
		public static readonly Gdk.Pixbuf IconCurveBezier;
		public static readonly Gdk.Pixbuf IconColorWheel;
		public static readonly Gdk.Pixbuf IconCancel;
		public static readonly Gdk.Pixbuf IconDelete;
		
		public static readonly Gdk.Cursor HandCursor;
		
		static Resources()
		{
			IconCurveDelete = Gdk.Pixbuf.LoadFromResource("chart_curve_delete.png");
			IconCurveBezier = Gdk.Pixbuf.LoadFromResource("chart_curve.png");
			IconCurveLinear = Gdk.Pixbuf.LoadFromResource("chart_line.png");
			IconColorWheel = Gdk.Pixbuf.LoadFromResource("color_wheel.png");
			IconCancel = Gdk.Pixbuf.LoadFromResource("cancel.png");
			IconDelete = Gdk.Pixbuf.LoadFromResource("delete.png");

			HandCursor = new Cursor (CursorType.Hand1);
		}
	}
}
