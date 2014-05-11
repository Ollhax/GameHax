using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using MG.Framework.Numerics;

using OpenTK.Graphics.OpenGL;

using Color = MG.Framework.Numerics.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = MG.Framework.Numerics.Rectangle;
using RectangleF = MG.Framework.Numerics.RectangleF;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// 2D texture representation.
	/// </summary>
	public class Texture2D : IDisposable
	{
		internal int textureId;
		internal OpenTK.Graphics.OpenGL.PixelFormat pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;

		/// <summary>
		/// Get the width of this texture.
		/// </summary>
		public int Width { get; protected set; }

		/// <summary>
		/// Get the height of this texture.
		/// </summary>
		public int Height { get; protected set; }
		
		/// <summary>
		/// Returns the size of this texture.
		/// </summary>
		public Vector2 Size { get { return new Vector2(Width, Height); } }

		/// <summary>
		/// Returns the bounds of this texture.
		/// </summary>
		public Rectangle Bounds { get { return new Rectangle(0, 0, Width, Height); } }

		/// <summary>
		/// Returns the bounds of this texture.
		/// </summary>
		public RectangleF BoundsF { get { return new RectangleF(0, 0, Width, Height); } }

		/// <summary>
		/// Load a texture from stream.
		/// </summary>
		/// <param name="stream">Memory stream of the bitmap file. Format should be BMP, GIF, EXIF, JPG, PNG or TIFF.</param>
		public Texture2D(Stream stream) { LoadFromStream(stream, true); }

		/// <summary>
		/// Load a texture from file.
		/// </summary>
		/// <remarks>This is the same as the constructor that takes a file. It's here only for XNA compatibility.</remarks>
		/// <param name="file">File to open. Format should be BMP, GIF, EXIF, JPG, PNG or TIFF.</param>
		public Texture2D(string file) { LoadFromFile(file, true); }

		/// <summary>
		/// Create a Texture2D object that should be initialized later on.
		/// </summary>
		internal Texture2D() { }

		/// <summary>
		/// Create an empty texture of the specified size.
		/// </summary>
		/// <param name="width">Width of texture.</param>
		/// <param name="height">Height of texture.</param>
		public Texture2D(int width, int height)
		{
			textureId = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureId);

			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba8,
				width,
				height,
				0,
				pixelFormat,
				PixelType.UnsignedByte,
				IntPtr.Zero);
			
			Width = width;
			Height = height;
		}
		
		/// <summary>
		/// Retrieve all the bits of this texture.
		/// </summary>
		/// <returns>An array of colors.</returns>
		public void GetData<T>(T[] data) where T: struct
		{
			int width;
			int height;
			int internalFormat;
			GL.BindTexture(TextureTarget.Texture2D, textureId);
			GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureInternalFormat, out internalFormat);
			GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out width);
			GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out height);
			
			int numBytes;
			OpenTK.Graphics.OpenGL.PixelFormat format;
			switch(internalFormat)
			{
				//case (int)PixelInternalFormat.Rgb:
				//    format = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
				//    numBytes = width * height * 3;
				//    break;

				case (int)PixelInternalFormat.Rgba8:
				case (int)PixelInternalFormat.Rgba:
					format = pixelFormat;
					numBytes = width * height * 4;
					break;

				default:
					throw new NotImplementedException("No support for format " + (PixelInternalFormat)internalFormat);
			}
			
			if (numBytes != 0)
			{
				GL.GetTexImage(TextureTarget.Texture2D, 0, format, PixelType.UnsignedByte, data);
			}
		}

		/// <summary>
		/// Set the color of this texture.
		/// </summary>
		/// <param name="data">Color to set. The array must be as large as this texture!</param>
		public void SetData<T>(T[] data) where T: struct
		{
			// TODO: Error checking?
			GL.BindTexture(TextureTarget.Texture2D, textureId);
			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba,
				Width,
				Height,
				0,
				OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
				PixelType.UnsignedByte,
				data);
		}

		/// <summary>
		/// Save this texture as a PNG.
		/// </summary>
		/// <param name="stream">Output stream.</param>
		public unsafe void SaveAsPng(Stream stream)
		{
			int w = Width;
			int h = Height;
			Color[] data = new Color[w * h];
			GetData(data);
			
			var bitmap = new Bitmap(w, h);
			var bmp_data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			for (var y = 0; y < bmp_data.Height; y++)
			{
				var rowPtr = (uint*)bmp_data.Scan0 + (y * bmp_data.Stride / 4);

				for (var x = 0; x < bmp_data.Width; x++)
				{
					var c = data[x + ((h - y - 1) * w)];
					var v = BitConverter.GetBytes(c.PackedValue);
					rowPtr[x] = (uint)(v[0] + (v[1] << 8) + (v[2] << 16) + (v[3] << 24));
				}
			}

			bitmap.UnlockBits(bmp_data);
			bitmap.Save(stream, ImageFormat.Png);
		}

		/// <summary>
		/// Retrieve the hash value.
		/// </summary>
		/// <returns>The hash value for this texture.</returns>
		public override int GetHashCode() { return textureId.GetHashCode(); }
		
		/// <summary>
		/// Load a texture from file.
		/// </summary>
		/// <remarks>This is the same as the constructor that takes a file. It's here only for XNA compatibility.</remarks>
		/// <param name="file">File to open. Format should be BMP, GIF, EXIF, JPG, PNG or TIFF.</param>
		/// <returns>A newly created texture.</returns>
		public static Texture2D FromFile(string file)
		{
			return new Texture2D(file);
		}

		public void Dispose()
		{
			if (textureId != 0)
			{
				GL.DeleteTexture(textureId);
			}
		}
		
		internal void LoadFromFile(string file, bool premultiplyAlpha)
		{
			using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				try
				{
					LoadFromStream(fs, premultiplyAlpha);
				}
				catch (ArgumentException)
				{
					throw new ArgumentException("Could not load file \"" + file + "\", probably not in a correct format (BMP, GIF, EXIF, JPG, PNG or TIFF).");
				}
			}
		}

		internal unsafe void LoadFromStream(Stream stream, bool premultiplyAlpha)
		{
			if (stream == null)
			{
				throw new ArgumentException("Null stream not allowed.");
			}

			var bmp = Image.FromStream(stream) as Bitmap;
			
			if (bmp == null)
			{
				throw new ArgumentException(
					"Input stream type not a valid bitmap. Format should be BMP, GIF, EXIF, JPG, PNG or TIFF.");
			}

			LoadFromBitmap(bmp, premultiplyAlpha);
		}

		internal unsafe void LoadFromBitmap(Bitmap bitmap, bool premultiplyAlpha)
		{
			if (textureId != 0)
			{
				GL.DeleteTexture(textureId);
			}

			textureId = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureId);

			BitmapData bmp_data = bitmap.LockBits(
				new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			if (premultiplyAlpha)
			{
				var inv255 = 1.0f / 255.0f;
				for (var y = 0; y < bmp_data.Height; y++)
				{
					var rowPtr = (byte*)bmp_data.Scan0 + (y * bmp_data.Stride);
					for (var x = 0; x < bmp_data.Width; x++)
					{
						var alpha = rowPtr[3 + (x * 4)];
						var i = x * 4;

						rowPtr[0 + i] = (byte)((float)rowPtr[0 + i] * alpha * inv255);
						rowPtr[1 + i] = (byte)((float)rowPtr[1 + i] * alpha * inv255);
						rowPtr[2 + i] = (byte)((float)rowPtr[2 + i] * alpha * inv255);
					}
				}
			}

			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba,
				bmp_data.Width,
				bmp_data.Height,
				0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, // Assume the input stream is in BGRA format
				PixelType.UnsignedByte,
				bmp_data.Scan0);

			bitmap.UnlockBits(bmp_data);

			Width = bitmap.Width;
			Height = bitmap.Height;
		}
	}
}