using System;
using System.Collections.Generic;
using System.IO;

using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Utility;

namespace MG.Framework.Assets
{
	public class AssetHandler
	{
		public readonly FilePath RootDirectory;

		private static readonly FilePath BaseLocation = AppDomain.CurrentDomain.BaseDirectory;
		private Dictionary<FilePath, object> cachedAssets = new Dictionary<FilePath, object>();
		private Texture2D dummyTexture;

		public AssetHandler(FilePath rootDirectory)
		{
			RootDirectory = rootDirectory;
			
			dummyTexture = new Texture2D(128, 128);
			var colorData = new Color[dummyTexture.Width * dummyTexture.Height];
			for (int i = 0; i < colorData.Length; i++)
			{
				colorData[i] = Color.Red;
			}
			dummyTexture.SetData(colorData);
		}

		public T Load<T>(FilePath asset)
		{
			T ret = default(T);
			
			// Check cache
			object cachedAsset;
			if (cachedAssets.TryGetValue(asset, out cachedAsset))
			{
				if (cachedAsset is T)
				{
					return (T)cachedAsset;
				}
			}
			
			// Try loading
			var fullPath = Path.Combine(BaseLocation, RootDirectory, asset);
			var obj = LoadInternal<T>(fullPath);
			if (obj != null)
			{
				cachedAssets[asset] = obj;
				return (T)obj;
			}

			return ret;
		}

		private object LoadInternal<T>(FilePath fullPath)
		{
			try
			{
				// TODO: This is pretty horrible (but good enough for now). Should be made more flexible.
				if (typeof(T) == typeof(Texture2D))
				{
					try
					{
						var texture = new Texture2D(fullPath);
						return texture;
					}
					catch (Exception)
					{
						return dummyTexture;
					}
				}
			}
			catch (Exception e)
			{
				throw new AssetLoadException("Could not load asset \"" + fullPath + "\".", e);
			}
			
			return null;
		}
	}
}
