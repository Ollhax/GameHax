using System.Runtime.InteropServices;
using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct Vertex2Color
	{
		public Vector2 Position;
		public Color Color;

		public Vertex2Color(Vector2 position, Color color)
		{
			Position = position;
			Color = color;
		}

		public static int GetSize()
		{
			return sizeof(float) * 2 + sizeof(byte) * 4;
		}
	}
}
