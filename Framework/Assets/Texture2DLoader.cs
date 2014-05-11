using System;
using System.Drawing;
using System.Drawing.Imaging;

using MG.Framework.Graphics;

using Color = MG.Framework.Numerics.Color;

namespace MG.Framework.Assets
{
	public class Texture2DLoader : IAssetLoader
	{
		public Type GetAssetType()
		{
			return typeof(Texture2D);
		}

		public object Create()
		{
			return new Texture2D();
		}

		public void Load(object asset, string filePath)
		{
			var texture = (Texture2D)asset;

			try
			{
				texture.LoadFromFile(filePath, true);
			}
			catch (Exception)
			{
				var bitmap = new Bitmap(128, 128);

				var bmp_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				
				unsafe
				{
					for (var y = 0; y < bmp_data.Height; y++)
					{
						var rowPtr = (byte*)bmp_data.Scan0 + (y * bmp_data.Stride);
						for (var x = 0; x < bmp_data.Width; x++)
						{
							var i = x * 4;
							rowPtr[0 + i] = 0;
							rowPtr[1 + i] = 0;
							rowPtr[2 + i] = 255;
							rowPtr[3 + i] = 255;
						}
					}
				}
				
				bitmap.UnlockBits(bmp_data);
				texture.LoadFromBitmap(bitmap, true);
			}
		}
	}
}
