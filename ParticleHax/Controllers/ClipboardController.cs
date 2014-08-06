using MG.ParticleEditorWindow;
using MG.ParticleHax.Actions;

namespace MG.ParticleHax.Controllers
{
	class ClipboardController
	{
		private MainController controller;
		private Model model;
		private Clipboard clipboard;

		public ClipboardController(MainController controller, Model model, Clipboard clipboard)
		{
			this.controller = controller;
			this.model = model;
			this.clipboard = clipboard;
		}

		public void Cut(int definitionId)
		{
			Copy(definitionId);
			
			var action = new RemoveAction(controller, model, definitionId);
			model.UndoHandler.ExecuteAction(action);
		}

		public void Copy(int definitionId)
		{
			var def = model.DefinitionTable.Definitions.GetById(definitionId);
			if (def == null) return;

			var d = DocumentSaver.Serialize(model, def);
			clipboard.Text = d;
		}

		public void Paste(int definitionId)
		{
			var text = clipboard.Text;
			if (string.IsNullOrEmpty(text)) return;
			
			var def = DocumentLoader.Deserialize(model, text);
			if (def == null) return;
			
			var action = new AddAction(controller, model, def, definitionId, true);
			model.UndoHandler.ExecuteAction(action);
		}
	}
}
