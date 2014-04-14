namespace MG.Framework.Graphics
{
	/// <summary>
	/// Controls the renderers texture sampling settings.
	/// </summary>
	public class TextureSampling
	{
		public enum TextureAddressMode
		{
			Clamp,
			Wrap
		}

		public enum TextureFilter
		{
			Linear,
			Nearest
		}

		public TextureFilter Filter;
		public TextureAddressMode AddressModeU;
		public TextureAddressMode AddressModeV;

		public static readonly TextureSampling LinearClamp = new TextureSampling { AddressModeU = TextureAddressMode.Clamp, AddressModeV = TextureAddressMode.Clamp, Filter = TextureFilter.Linear };
		public static readonly TextureSampling NearestClamp = new TextureSampling { AddressModeU = TextureAddressMode.Clamp, AddressModeV = TextureAddressMode.Clamp, Filter = TextureFilter.Nearest };
		public static readonly TextureSampling LinearWrap = new TextureSampling { AddressModeU = TextureAddressMode.Wrap, AddressModeV = TextureAddressMode.Wrap, Filter = TextureFilter.Linear };
		public static readonly TextureSampling NearestWrap = new TextureSampling { AddressModeU = TextureAddressMode.Wrap, AddressModeV = TextureAddressMode.Wrap, Filter = TextureFilter.Nearest };
	}
}
