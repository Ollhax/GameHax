namespace MG.ParticleEditorWindow
{
	public class Clipboard
	{
		private Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));

		public string Text
		{
			get { return clipboard.WaitForText(); }
			set { clipboard.Text = value; }
		}
	}
}
