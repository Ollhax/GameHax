using System;

using MG.Framework.Particle;
using MG.ParticleEditorWindow;

namespace MG.ParticleEditor.Controllers
{
	class DocumentController
	{
		private Model model;

		public event Action NewDocument = delegate { };

		public DocumentController(Model model, MainWindow window)
		{
			this.model = model;
			window.FileNew += New;
			window.FileOpen += Open;

			window.EditUndo += Undo;
			window.EditRedo += Redo;
		}

		public void New()
		{
			model.DefinitionTable = new ParticleDefinitionTable();
			NewDocument.Invoke();
		}

		public void Open()
		{
			
		}

		public void Undo()
		{
			model.UndoHandler.Undo();
		}

		public void Redo()
		{
			model.UndoHandler.Redo();
		}

		//public void Close()
		//{

		//}
	}
}
