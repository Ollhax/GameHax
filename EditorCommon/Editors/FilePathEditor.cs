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
			var declarationParameter = ((ParticleParameterDescriptor)Property).DeclarationParameter;

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

			var filters = GtkTools.ParseFilterString(declarationParameter.FilePathFilter);

			if (filters != null)
			{
				filters.ForEach(fc.AddFilter);
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