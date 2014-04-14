using System.Runtime.InteropServices;
using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	struct Vertex2Tex2Color
	{
		public Vector2 Position;
		public Color Color;
		public Vector2 TextureCoordinate;

		public Vertex2Tex2Color(Vector2 position, Color color, Vector2 texCoord)
		{
			Position = position;
			Color = color;
			TextureCoordinate = texCoord;
		}

		public static int GetSize()
		{
			return sizeof(float) * 4 + sizeof(byte) * 4;
		}
	}
}
