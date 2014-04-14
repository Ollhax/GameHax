using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MG.Framework.Utility
{
	/// <summary>
	/// Misc file path related helpers.
	/// </summary>
	public static class PathHelper
	{		
		//---------------------------------------------------------------//
		#region Methods
		//---------------------------------------------------------------//
		public static readonly string PreferedSlash = @"\";
		public static readonly string UnpreferedSlash = @"/";

		/// <summary>
		/// Convert the target path to the preferred string format.
		/// This method does not care about locale.
		/// </summary>
		/// <param name="path">Path to convert.</param>
		/// <returns>Output path.</returns>
		public static string CanonicalPath(string path)
		{			
			path = path.Trim();
			path = path.Replace(UnpreferedSlash, PreferedSlash);
			
			// Remove any redundant slashes at beginning
			while (path.Length > 0 && path[0] == PreferedSlash[0])
			{
				path = path.Substring(1);
			}

			return path;
		}

		/// <summary>
		/// Retrieves a copy of the path with a slash at the end. 
		/// Will not add a slash if there already is one there.
		/// </summary>
		/// <param name="path">Path to fix.</param>
		/// <returns>Path with slash at the end.</returns>
		public static string EnsureTrailingSlash(string path)
		{
			if (path.Length > 0 && path[path.Length - 1] != PreferedSlash[0])
			{
				return path + PreferedSlash;
			}
			return path;
		}

		/// <summary>
		/// Retrieves a copy of the path without a slash at the end. 
		/// </summary>
		/// <param name="path">Path to fix.</param>
		/// <returns>Path without slash at the end.</returns>
		public static string EnsureNoTrailingSlash(string path)
		{
			if (path.Length > 0 && path[path.Length - 1] == PreferedSlash[0])
			{
				return path.Substring(0, path.Length - 1);
			}
			return path;
		}

		/// <summary>
		/// Retrieves a copy of the path with a certain extension. 
		/// If the extension is there, no change will be made. If it's the wrong one, it will be changed.
		/// </summary>
		/// <param name="path">Path to fix.</param>
		/// <param name="extension">Extension to add to the end if it does not exist. Should be specified without dot, e.g. "png". </param>
		/// <returns>Path with the specified extension.</returns>
		public static string EnsureFileExtension(string path, string extension)
		{
			if (path.EndsWith("." + extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return path;
			}

			return GetFileNameWithoutExtension(path) + "." + extension;
		}

		/// <summary>
		/// Robust path comparison.
		/// </summary>
		/// <returns>True if the two paths are equal.</returns>
		public static bool SameFilePath(string firstPath, string secondPath)
		{
			return CanonicalPath(firstPath) == CanonicalPath(secondPath);
		}

		/// <summary>
		/// Returns a filename without path information.
		/// </summary>
		public static string GetFileNameWithoutPath(string path)
		{
			int index = path.LastIndexOf(Path.DirectorySeparatorChar);
			if (index > 0)
			{
				path = path.Substring(index + 1);
			}

			return path;			
		}

		/// <summary>
		/// Returns a filename without extension.
		/// </summary>
		public static string GetFileNameWithoutExtension(string path)
		{
			int index = path.LastIndexOf('.');
			if (index > 0)
			{
				path = path.Substring(0, index);
			}

			return path;		
		}

		/// <summary>
		/// Returns a filename without path or extension.
		/// </summary>
		public static string GetFileNameWithoutPathOrExtension(string path)
		{
			return GetFileNameWithoutPath(Path.GetFileNameWithoutExtension(path));
		}
		
		/// <summary>
		/// Returns the name of the top-most folder.
		/// </summary>
		public static string GetFolderNameWithoutPath(string path)
		{
			path = EnsureNoTrailingSlash(path);

			var index = path.LastIndexOf(Path.DirectorySeparatorChar);
			if (index > 0)
			{
				path = path.Substring(index + 1);
			}

			return path;
		}

		/// <summary>
		/// Go up N folders in the path hierarchy. 
		/// Will first strip any file/extension from the path.
		/// Does not remove the trailing slash from the root.
		/// </summary>
		/// <param name="path">Path to strip of folders.</param>
		/// <param name="levels">Number of levels to strip. Usually 1.</param>
		/// <returns>Path that has been stripped.</returns>
		public static string StripFolders(string path, int levels)
		{
			if (Path.HasExtension(path))
			{
				path = Path.GetDirectoryName(path);
			}
			
			string root = "";
			if (Path.IsPathRooted(path))
			{
				root = Path.GetPathRoot(path);
			}

			path = EnsureNoTrailingSlash(path);
			
			for (var i = 0; i < levels; i++)
			{
				var index = path.LastIndexOf(Path.DirectorySeparatorChar);
				if (index > 0)
				{
					path = path.Substring(0, index);
				}
				else
				{
					break;
				}
			}

			// At root directory?
			if (path.Length < root.Length)
			{
				return root;
			}

			return path;
		}

		/// <summary>
		/// Ensure that the target path exists. Creates subdirectories as required.
		/// </summary>
		/// <param name="path">Path to create, if non-existent.</param>
		public static void EnsurePathExists(string path)
		{
			path = Path.GetFullPath(path);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static string GetRelativePath(string sourceFilePath, string targetFilePath)
		{
			// Special case: equal paths
			if (sourceFilePath == targetFilePath)
				return "";

			// Tokenize path. We use regex split with parantheses in order to keep the tokens.
			var pattern = "(" + Regex.Escape(PreferedSlash) + ")";

			if (Path.HasExtension(sourceFilePath))
			{
				sourceFilePath = (sourceFilePath.Length > 0 ? (Path.GetDirectoryName(sourceFilePath)) : "");
			}
			sourceFilePath = EnsureTrailingSlash(sourceFilePath);

			var source = Regex.Split(sourceFilePath, pattern).Where(p => p.Length > 0).ToArray();
			var target = Regex.Split(targetFilePath, pattern).Where(p => p.Length > 0).ToArray();
			
			// Find common start
			uint common;
			for (common = 0; common < Math.Min(source.Length, target.Length); common++)
			{
				if (source[common] != target[common])
				{
					break;
				}
			}
		
			// Find prefix
			var prefix = "";
		
			for (uint i = common; i < source.Length; i++)
			{
				if (source[i] != PreferedSlash)
				{
					if (string.IsNullOrEmpty(prefix))
					{
						prefix += "..";
					}
					else
					{
						prefix += PreferedSlash + "..";
					}
				}
			}
		
			// Find suffix
			var suffix = "";
		
			for (uint i = common; i < target.Length; i++)
			{
				suffix += target[i];
			}

			return Path.Combine(prefix, suffix);
		}
		
		#endregion
		//---------------------------------------------------------------//
	}
}
