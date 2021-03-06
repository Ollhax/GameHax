using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using MG.Framework.Converters;

namespace MG.Framework.Utility
{
	/// <summary>
	/// A representation of a system path to a file or directory.
	/// </summary>
	[Serializable, TypeConverter(typeof(FilePathConverter))]
	public struct FilePath : IComparable<FilePath>, IComparable, IEquatable<FilePath>, ICustomTypeDescriptor
	{
		// License note: Source code originates from MonoDevelop. See license at the bottom of this class.

		private static readonly StringComparer PathComparer = (Platform.IsWindows || Platform.IsMac)
		                                                      	? StringComparer.OrdinalIgnoreCase
		                                                      	: StringComparer.Ordinal;

		private static readonly StringComparison PathComparison = (Platform.IsWindows || Platform.IsMac)
		                                                          	? StringComparison.OrdinalIgnoreCase
		                                                          	: StringComparison.Ordinal;

		private readonly string fileName;

		public static readonly FilePath Null = new FilePath(null);
		public static readonly FilePath Empty = new FilePath(string.Empty);

		public FilePath(string name)
		{
			fileName = name;
		}

		public bool IsNull { get { return fileName == null; } }

		public bool IsNullOrEmpty { get { return string.IsNullOrEmpty(fileName); } }

		public bool IsNotNull { get { return fileName != null; } }

		public bool IsEmpty { get { return fileName != null && fileName.Length == 0; } }

		public FilePath FullPath { get { return new FilePath(!string.IsNullOrEmpty(fileName) ? Path.GetFullPath(fileName) : ""); } }

		public bool IsDirectory { get { return File.GetAttributes(FullPath).HasFlag(FileAttributes.Directory); } }

		public string String { get { return fileName; } }

		/// <summary>
		/// Returns a path in standard form, which can be used to be compared
		/// for equality with other canonical paths. It is similar to FullPath,
		/// but unlike FullPath, the directory "/a/b" is considered equal to "/a/b/"
		/// </summary>
		public FilePath CanonicalPath
		{
			get
			{
				if (string.IsNullOrEmpty(fileName))
				{
					return FilePath.Empty;
				}

				string fp = Path.GetFullPath(fileName);
				if (fp.Length > 0 && fp[fp.Length - 1] == Path.DirectorySeparatorChar)
				{
					return fp.TrimEnd(Path.DirectorySeparatorChar);
				}

				if (fp.Length > 0 && fp[fp.Length - 1] == Path.AltDirectorySeparatorChar)
				{
					return fp.TrimEnd(Path.AltDirectorySeparatorChar);
				}

				return fp;
			}
		}

		public string FileName { get { return Path.GetFileName(fileName); } }

		public string Extension { get { return Path.GetExtension(fileName); } }

		public bool HasExtension(string extension)
		{
			return fileName.Length > extension.Length && fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
			       && fileName[fileName.Length - extension.Length - 1] != Path.PathSeparator;
		}

		public string FileNameWithoutExtension { get { return Path.GetFileNameWithoutExtension(fileName); } }

		public FilePath ParentDirectory { get { return new FilePath(Path.GetDirectoryName(fileName)); } }

		public bool IsAbsolute { get { return Path.IsPathRooted(fileName); } }

		public bool IsChildPathOf(FilePath basePath)
		{
			if (basePath.fileName[basePath.fileName.Length - 1] != Path.DirectorySeparatorChar)
			{
				return fileName.StartsWith(basePath.fileName + Path.DirectorySeparatorChar, PathComparison);
			}
			else
			{
				return fileName.StartsWith(basePath.fileName, PathComparison);
			}
		}

		public FilePath ChangeExtension(string ext)
		{
			return Path.ChangeExtension(fileName, ext);
		}

		public FilePath Combine(params FilePath[] paths)
		{
			string path = fileName;
			foreach (FilePath p in paths)
			{
				path = Path.Combine(path, p.fileName);
			}
			return new FilePath(path);
		}

		public FilePath Combine(params string[] paths)
		{
			string path = fileName;
			foreach (string p in paths)
			{
				path = Path.Combine(path, p);
			}
			return new FilePath(path);
		}

		public void Delete()
		{
			// Ensure that this file/directory and all children are writable
			MakeWritable(true);

			// Also ensure the directory containing this file/directory is writable,
			// otherwise we will not be able to delete it
			ParentDirectory.MakeWritable(false);

			if (Directory.Exists(this))
			{
				Directory.Delete(this, true);
			}
			else if (File.Exists(this))
			{
				File.Delete(this);
			}
		}

		public void MakeWritable()
		{
			MakeWritable(false);
		}

		public void MakeWritable(bool recurse)
		{
			if (Directory.Exists(this))
			{
				try
				{
					var info = new DirectoryInfo(this);
					info.Attributes &= ~FileAttributes.ReadOnly;
				}
				catch
				{
				}

				if (recurse)
				{
					foreach (var sub in Directory.GetFileSystemEntries(this))
					{
						((FilePath)sub).MakeWritable(recurse);
					}
				}
			}
			else if (File.Exists(this))
			{
				try
				{
					// Try/catch is to work around a mono bug where dangling symlinks
					// blow up when you call SetFileAttributes. Just ignore this case
					// until mono 2.10.7/8 is released which fixes it.
					var info = new FileInfo(this);
					info.Attributes &= ~FileAttributes.ReadOnly;
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Builds a path by combining all provided path sections
		/// </summary>
		public static FilePath Build(params string[] paths)
		{
			return Empty.Combine(paths);
		}

		public static FilePath GetCommonRootPath(IEnumerable<FilePath> paths)
		{
			FilePath root = FilePath.Null;
			foreach (FilePath p in paths)
			{
				if (root.IsNull)
				{
					root = p;
				}
				else if (root == p)
				{
					continue;
				}
				else if (root.IsChildPathOf(p))
				{
					root = p;
				}
				else
				{
					while (!root.IsNullOrEmpty && !p.IsChildPathOf(root))
					{
						root = root.ParentDirectory;
					}
				}
			}
			return root;
		}

		public FilePath ToAbsolute(FilePath basePath)
		{
			if (IsAbsolute)
			{
				return FullPath;
			}
			else
			{
				return Combine(basePath, this).FullPath;
			}
		}

		public FilePath ToRelative(FilePath basePath)
		{
			return AbsoluteToRelativePath(basePath, fileName);
		}
		
		public static implicit operator FilePath(string name)
		{
			return new FilePath(name);
		}

		public static implicit operator string(FilePath filePath)
		{
			return filePath.fileName;
		}

		public static bool operator ==(FilePath name1, FilePath name2)
		{
			return PathComparer.Equals(name1.fileName, name2.fileName);
		}

		public static bool operator !=(FilePath name1, FilePath name2)
		{
			return !(name1 == name2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FilePath))
			{
				return false;
			}

			FilePath fn = (FilePath)obj;
			return this == fn;
		}

		public override int GetHashCode()
		{
			if (fileName == null)
			{
				return 0;
			}

			return PathComparer.GetHashCode(fileName);
		}

		public override string ToString()
		{
			return fileName;
		}

		public int CompareTo(FilePath filePath)
		{
			return PathComparer.Compare(fileName, filePath.fileName);
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is FilePath))
			{
				return -1;
			}
			return CompareTo((FilePath)obj);
		}

		bool IEquatable<FilePath>.Equals(FilePath other)
		{
			return this == other;
		}

		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.VolumeSeparatorChar, Path.AltDirectorySeparatorChar };
		private static string AbsoluteToRelativePath(string baseDirectoryPath, string absPath)
		{
			if (!Path.IsPathRooted(absPath))
				return absPath;

			absPath = Path.GetFullPath(absPath); // Crashes on Mono if working directory is at the root
			baseDirectoryPath = Path.GetFullPath(baseDirectoryPath.TrimEnd(Path.DirectorySeparatorChar));

			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;

			for (; indx < Math.Min(bPath.Length, aPath.Length); indx++)
			{
				if (!bPath[indx].Equals(aPath[indx]))
					break;
			}

			if (indx == 0)
				return absPath;

			StringBuilder result = new StringBuilder();

			for (int i = indx; i < bPath.Length; i++)
			{
				result.Append("..");
				if (i + 1 < bPath.Length || aPath.Length - indx > 0)
					result.Append(Path.DirectorySeparatorChar);
			}

			result.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
			if (result.Length == 0)
				return ".";
			return result.ToString();
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties(null);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var properties = new List<PropertyDescriptor>();
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this.GetType(), attributes);

			properties.Add(pdc["String"]);

			//foreach (PropertyDescriptor oPropertyDescriptor in pdc)
			//{
			//    properties.Add(oPropertyDescriptor);
			//}
			
			return new PropertyDescriptorCollection(properties.ToArray());
		}
	}
	
	// Original license:
	// 
	// FilePath.cs
	//  
	// Author:
	//       Lluis Sanchez Gual <lluis@novell.com>
	// 
	// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
	// 
	// Permission is hereby granted, free of charge, to any person obtaining a copy
	// of this software and associated documentation files (the "Software"), to deal
	// in the Software without restriction, including without limitation the rights
	// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	// copies of the Software, and to permit persons to whom the Software is
	// furnished to do so, subject to the following conditions:
	// 
	// The above copyright notice and this permission notice shall be included in
	// all copies or substantial portions of the Software.
	// 
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	// THE SOFTWARE.
}