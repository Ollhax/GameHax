using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using MG.Framework.Utility;

namespace MG.Framework.Assets
{
	class AssetWatcher
	{
		public readonly FilePath RootDirectory;
		public readonly FilePath WatchDirectory;
		
		private FileSystemWatcher watcher;

		private HashSet<FilePath> changedFiles = new HashSet<FilePath>();
		private Stopwatch lastChangeTime = new Stopwatch();
		
		public AssetWatcher(FilePath rootDirectory, FilePath watchDirectory)
		{
			RootDirectory = rootDirectory;
			WatchDirectory = watchDirectory;

			watcher = new FileSystemWatcher(watchDirectory.FullPath);
			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			watcher.Changed += OnFilesChanged;
			watcher.Created += OnFilesCreated;
			watcher.Deleted += OnFilesDeleted;
			watcher.Renamed += OnFilesRenamed;
			watcher.IncludeSubdirectories = false;
			watcher.EnableRaisingEvents = true;

			lastChangeTime.Start();
		}

		public HashSet<FilePath> GetChangedFiles()
		{
			lock (changedFiles)
			{
				// Wait a few moments before reporting the change, as multiple change events may trigger at once.
				// Note: If events are triggered all the time, the timer will be reset and the changes will never
				// be reported. No problem for now though.
				if (lastChangeTime.Elapsed.TotalMilliseconds < 200)
					return null;

				if (changedFiles.Count > 0)
				{
					var copy = new HashSet<FilePath>(changedFiles);
					changedFiles.Clear();
					return copy;
				}
			}

			return null;
		}

		private void ListFileChange(FilePath file)
		{
			lock (changedFiles)
			{
				var relativePath = file.ToRelative(RootDirectory);

				changedFiles.Add(relativePath);
				lastChangeTime.Restart();
			}
		}

		private void OnFilesRenamed(object sender, RenamedEventArgs renamedEventArgs)
		{
			//Log.P("File renamed: " + renamedEventArgs.OldFullPath + " to " + renamedEventArgs.FullPath);
			ListFileChange(renamedEventArgs.OldFullPath);
			ListFileChange(renamedEventArgs.FullPath);
		}

		private void OnFilesDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			//Log.P("File deleted: " + fileSystemEventArgs.Name);
			ListFileChange(fileSystemEventArgs.FullPath);
		}

		private void OnFilesCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			//Log.P("File created: " + fileSystemEventArgs.Name);
			ListFileChange(fileSystemEventArgs.FullPath);
		}

		private void OnFilesChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			//Log.P("File changed: " + fileSystemEventArgs.Name + " (" + fileSystemEventArgs.ChangeType + ")");
			ListFileChange(fileSystemEventArgs.FullPath);
		}
	}
}
