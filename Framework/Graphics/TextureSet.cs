using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using MG.Framework.Numerics;
using MG.Framework.Utility;
using MG.Framework.Converters;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// A generic set of textures, primarily used for storing atlas(es).
	/// </summary>
	public class TextureSet
	{
		private struct TextureData
		{
			public RectangleF Area;
			public Texture2D Texture;

			private TextureData(Texture2D texture, RectangleF area)
			{
				Texture = texture;
				Area = area;
			}
		};

		private readonly Dictionary<string, TextureData> textureDataSet = new Dictionary<string, TextureData>();
		
		/// <summary>
		/// Create a texture set as an atlas.
		/// </summary>
		/// <param name="in_pairs">A list of image-map pairs.</param>
		public TextureSet(List<Tuple<FilePath, FilePath>> imageMapPairs)
		{
			foreach (var pair in imageMapPairs)
			{
				string image = pair.Item1;
				string map = pair.Item2;

				// Load texture
				var texture = new Texture2D(image);

				// Load & parse map
				float divisor = 1; //Globals::GetHiResContentScale();
				IEnumerable<string> lines = File.ReadLines(map);

				Debug.Assert(lines != null, "Map file non-existant or empty: " + map);

				foreach (string line in lines)
				{
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}

					string[] tokens = line.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

					Debug.Assert(tokens.Length == 2, "Invalid token count in map " + map + ", line: " + line);
					string idString = tokens[0].Trim();
					string rectString = tokens[1].Trim();

					string[] rectParts = rectString.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					Debug.Assert(rectParts.Length == 4, "Invalid rectangle token count in map " + map + ", line: " + line);

					string file;
					RectangleF rect;

					file = idString;
					TypeConvert.FromString(rectParts[0], out rect.X);
					TypeConvert.FromString(rectParts[1], out rect.Y);
					TypeConvert.FromString(rectParts[2], out rect.Width);
					TypeConvert.FromString(rectParts[3], out rect.Height);
					rect.X /= divisor;
					rect.Y /= divisor;
					rect.Width /= divisor;
					rect.Height /= divisor;

					Debug.Assert(!textureDataSet.ContainsKey(file));
					textureDataSet.Add(file, new TextureData { Area = rect, Texture = texture });
				}
			}
		}

		/// <summary>
		/// Create a set from a group of images
		/// </summary>
		/// <param name="images">Images to create a set from.</param>
		public TextureSet(IEnumerable<FilePath> images)
		{
			float divisor = 1.0f; //; Globals::GetCurrentContentScale();

			// Load textures
			foreach (string path in images)
			{
				var texture = new Texture2D(path);

				string fileId = Path.GetFileNameWithoutExtension(path);
				Vector2 size = texture.Size;
				var rect = new RectangleF(0, 0, size.X / divisor, size.Y / divisor);

				Debug.Assert(!textureDataSet.ContainsKey(fileId));
				textureDataSet.Add(fileId, new TextureData { Area = rect, Texture = texture });
			}
		}

		/// <summary>
		/// Fetch a named texture from this set.
		/// </summary>
		/// <param name="textureName">Name of texture.</param>
		/// <param name="texture">Outputted texture.</param>
		/// <returns>True if the texture exists.</returns>
		public bool GetTexture(string textureName, out TextureSetReference texture)
		{
			TextureData textureData;
			if (textureDataSet.TryGetValue(textureName, out textureData))
			{
				texture = new TextureSetReference(textureName, "", textureData.Texture, textureData.Area); // TODO: Set name?
				return true;
			}

			texture = new TextureSetReference();
			return false;
		}

		public void Dispose()
		{
			foreach (var textureData in textureDataSet)
			{
				textureData.Value.Texture.Dispose();
			}
		}

		public static TextureSet CreateFromDirectory(FilePath directory)
		{
			string[] files = Directory.GetFiles(directory, "*.txt");
			
			// Atlases
			var atlasPairs = new List<Tuple<FilePath, FilePath>>();

			foreach (var file in files)
			{
				var atlasPath = new FilePath(file);
				var texPath = atlasPath.ChangeExtension("png");

				atlasPairs.Add(new Tuple<FilePath, FilePath>(texPath, atlasPath));
			}

			// Load atlas
			return new TextureSet(atlasPairs);
		}

		public static TextureSet CreateFromListFile(FilePath atlasListFile)
		{
			string spriteSetFolder = Path.GetDirectoryName(atlasListFile);

			string[] fileList = File.ReadAllLines(atlasListFile);

			if (fileList[0] == "atlases:")
			{
				// Atlases
				var atlasPairs = new List<Tuple<FilePath, FilePath>>();

				for (uint i = 1; i < fileList.Length; ++i)
				{
					FilePath atlasPath = Path.Combine(spriteSetFolder, fileList[i]);
					FilePath texPath = atlasPath.ChangeExtension("png");

					atlasPairs.Add(new Tuple<FilePath, FilePath>(texPath, atlasPath));
				}

				// Load atlas
				return new TextureSet(atlasPairs);
			}
			else
			{
				// Separate textures
				//foreach(string texPath in fileList)
				//{
				//    texPath = Path.Combine(spriteSetFolder, texPath);
				//}

				var paths = new FilePath[fileList.Length];
				for (int i = 0; i < fileList.Length; i++)
				{
					paths[i] = fileList[i];
				}

				return new TextureSet(paths);
			}
		}
	};
}