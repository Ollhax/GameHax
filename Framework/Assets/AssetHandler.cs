using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MG.Framework.Utility;

namespace MG.Framework.Assets
{
	public class AssetHandler
	{
		public readonly FilePath RootDirectory;
		public event Action<FilePath> AssetChanged;

		private static readonly FilePath baseLocation = AppDomain.CurrentDomain.BaseDirectory;
		private Dictionary<FilePath, object> cachedAssets = new Dictionary<FilePath, object>();
		private Dictionary<FilePath, AssetWatcher> watchers = new Dictionary<FilePath, AssetWatcher>();
		private Dictionary<Type, IAssetLoader> assetLoaders = new Dictionary<Type, IAssetLoader>();
		
		public AssetHandler(FilePath rootDirectory)
		{
			RootDirectory = rootDirectory;

			var type = typeof(IAssetLoader);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(type.IsAssignableFrom);

			foreach (var t in types)
			{
				if (t.IsAbstract) continue;
				var ctor = t.GetConstructor(new Type[] { });
				if (ctor == null) continue;
				
				var loader = (IAssetLoader)ctor.Invoke(new object[] { });
				assetLoaders.Add(loader.GetAssetType(), loader);
			}
		}

		public FilePath GetFullPath(FilePath assetPath)
		{
			return Path.Combine(RootDirectory, assetPath);
		}
		
		public void Update()
		{
			foreach (var watcher in watchers)
			{
				var files = watcher.Value.GetChangedFiles();
				if (files != null)
				{
					foreach (var file in files)
					{
						object asset;
						if (cachedAssets.TryGetValue(file, out asset))
						{
							var fullPath = file.FullPath;
							Log.Info("Reloading: " + fullPath);
							OnAssetChanged(file);
							Reload(fullPath, asset);
						}
					}
				}
			}
		}

		public T Load<T>(FilePath asset)
		{
			T ret = default(T);
			var assetFullPath = ((FilePath)(Path.Combine(RootDirectory, asset))).FullPath;
			var assetDirectory = assetFullPath.ParentDirectory;
			var assetCanonicalDirectoryPath = assetDirectory.CanonicalPath;
			var assetName = assetFullPath.ToRelative(RootDirectory);

			AssetWatcher watcher;
			if (!watchers.TryGetValue(assetCanonicalDirectoryPath, out watcher))
			{
				Log.Info("Creating watcher for directory: " + assetCanonicalDirectoryPath);
				watcher = new AssetWatcher(RootDirectory, assetCanonicalDirectoryPath);
				watchers.Add(assetCanonicalDirectoryPath, watcher);
			}

			// Check cache
			object cachedAsset;
			if (cachedAssets.TryGetValue(assetName, out cachedAsset))
			{
				if (cachedAsset is T)
				{
					return (T)cachedAsset;
				}
			}

			// Try loading
			var fullPath = Path.Combine(baseLocation, RootDirectory, assetName);
			Log.Info("Loading " + typeof(T).Name + " from file " + fullPath);

			var obj = LoadInternal<T>(fullPath);
			if (obj != null)
			{
				cachedAssets[assetName] = obj;
				return (T)obj;
			}

			Log.Info("- Failure!");
			return ret;
		}
		
		private object LoadInternal<T>(FilePath fullPath)
		{
			IAssetLoader loader;
			if (assetLoaders.TryGetValue(typeof(T), out loader))
			{
				var asset = loader.Create();
				loader.Load(asset, fullPath);
				return asset;
			}

			throw new ArgumentException("Error loading \"" + fullPath + "\". Unregistered asset type: " + typeof(T));
		}

		private void OnAssetChanged(FilePath fullPath)
		{
			if (AssetChanged != null)
			{
				AssetChanged.Invoke(fullPath);
			}
		}

		private void Reload(FilePath fullPath, object asset)
		{
			IAssetLoader loader;
			if (assetLoaders.TryGetValue(asset.GetType(), out loader))
			{
				loader.Load(asset, fullPath);
			}
		}
	}
}
