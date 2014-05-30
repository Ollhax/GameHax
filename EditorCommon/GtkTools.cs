using System;
using System.Collections.Generic;

using Gtk;

namespace MG.EditorCommon
{
	public static class GtkTools
	{
		public static List<FileFilter> ParseFilterString(string filterText)
		{
			if (string.IsNullOrEmpty(filterText)) return null;

			var filters = new List<FileFilter>();
			var parts = filterText.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i + 1 < parts.Length; i += 2)
			{
				var filter = new FileFilter();
				filter.Name = parts[i];

				var patterns = parts[i + 1].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < patterns.Length; j++)
				{
					filter.AddPattern(patterns[j]);
				}

				filters.Add(filter);
			}
			return filters;
		}
	}
}
