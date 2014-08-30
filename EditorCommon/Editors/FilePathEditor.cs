using System;
using System.Linq;

using Gdk;

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

		protected override string GetValueText()
		{
			if (Instance == null) return "";
			var currentValue = (FilePath)Value;

			return currentValue.ToRelative(Environment.CurrentDirectory);
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
			fc.PreviewWidget = previewImage;
			fc.UpdatePreview += DialogOnUpdatePreview;
			UpdatePreview(fc, currentValue);
			
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

		private Gtk.Image previewImage = new Gtk.Image();

		private void DialogOnUpdatePreview(object sender, EventArgs eventArgs)
		{
			var dialog = sender as FileChooserDialog;
			if (dialog == null) return;

			UpdatePreview(dialog, dialog.PreviewFilename);
		}

		private void UpdatePreview(FileChooserDialog dialog, string file)
		{
			try
			{
				previewImage.Pixbuf = new Pixbuf(file, 128, 128);
				dialog.PreviewWidgetActive = true;
			}
			catch (Exception)
			{
				dialog.PreviewWidgetActive = false;
			}
		}
	}

	public class FilePathIsFolderAttribute : Attribute
	{

	}
}