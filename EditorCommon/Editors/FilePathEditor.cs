using System;
using System.Linq;
using Gtk;

using MG.Framework.Utility;

using MonoDevelop.Components.PropertyGrid;

namespace MG.EditorCommon.Editors
{
	public class FilePathEditor : PropertyEditorCell
	{
		public override bool DialogueEdit
		{
			get { return true; }
		}
		
		public override void LaunchDialogue()
		{
			var declarationParameter = ((AnyPropertyDescriptor)Property).DeclarationParameter;

			var kindAtt = this.Property.Attributes.OfType<FilePathIsFolderAttribute>().FirstOrDefault();
			FileChooserAction action;
			string title;
			if (kindAtt == null)
			{
				action = FileChooserAction.Open;
				title = "Select File...";
			}
			else
			{
				action = FileChooserAction.SelectFolder;
				title = "Select Folder...";
			}

			var currentValue = (FilePath)Value;
			
			var fc = new FileChooserDialog(title, null, action, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			//fc.SetCurrentFolder(currentValue.ParentDirectory);
			fc.SetFilename(currentValue);

			var filterText = declarationParameter.FilePathFilter;
			if (filterText != null)
			{
				var parts = filterText.Split(new [] { "|" }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i + 1 < parts.Length; i += 2)
				{
					var filter = new FileFilter();
					filter.Name = parts[i];

					var patterns = parts[i+1].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < patterns.Length; j++)
					{
						filter.AddPattern(patterns[j]);
					}
					
					fc.AddFilter(filter);
				}
			}

			if (fc.Run() == (int)ResponseType.Accept)
			{
				Property.SetValue(Instance, (FilePath)fc.Filename);
			}

			fc.Destroy();
		}
	}

	public class FilePathIsFolderAttribute : Attribute
	{

	}
}