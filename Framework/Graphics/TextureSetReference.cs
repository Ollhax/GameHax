using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// A reference to a texture set instance.
	/// </summary>
	public struct TextureSetReference
	{
		public string TextureName;
		public string SetName;
		public Texture2D Texture;
		public RectangleF Area;

		public TextureSetReference(string textureName, string setName, Texture2D texture, RectangleF area)
		{
			TextureName = textureName;
			SetName = setName;
			Texture = texture;
			Area = area;
		}
	};
}
