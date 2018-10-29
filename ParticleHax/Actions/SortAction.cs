using System;
using System.Collections.Generic;

using MG.ParticleHax.Controllers;
using MG.Framework.Utility;
using MG.Framework.Particle;

namespace MG.ParticleHax.Actions
{
	class SortAction : UndoableParticleAction
	{
		private MainController controller;
		private Model model;
		private int[] oldOrder;

		public SortAction(MainController controller, Model model)
		{
			this.controller = controller;
			this.model = model;

			var collection = this.model.DefinitionTable.Definitions;
			this.oldOrder = new int[collection.Count];

			var index = 0;
			foreach (var def in collection)
			{
				oldOrder[index] = def.Id;
				index++;
			}
			//Log.Info("Saved order: " + string.Join(", ", oldOrder));
		}

		protected override bool CallExecute()
		{
			//Log.Info("Sort by name");
			var collection = this.model.DefinitionTable.Definitions;
			collection.SortByName();

			// Check that the order actually changed by comparing to saved id list.
			var index = 0;
			foreach (var def in collection)
			{
				if (def.Id != oldOrder[index])
				{
					// Found change.
					controller.UpdateTree = true;
					model.Modified = true;
					return true;
				}
				index++;
			}

			return false;
		}

		protected override void CallUndo()
		{
			//Log.Info("Sort by old id order");
			var collection = this.model.DefinitionTable.Definitions;
			var comparer = new IdComparer(this.oldOrder);
			collection.Sort(comparer);
			controller.UpdateTree = true;
			model.Modified = true;
		}
	}

	public class IdComparer : Comparer<ParticleDefinition>
	{
		private int[] idList;

		public IdComparer(int[] idList)
		{
			this.idList = idList;
		}

		public override int Compare(ParticleDefinition a, ParticleDefinition b)
		{
			var indexOfA = Array.IndexOf(this.idList, a.Id);
			var indexOfB = Array.IndexOf(this.idList, b.Id);

			if (indexOfA == -1)
			{
				throw new ApplicationException("Can not find definition " + a.Id);
			}

			if (indexOfB == -1)
			{
				throw new ApplicationException("Can not find definition " + b.Id);
			}

			return indexOfA - indexOfB;
		}
	}
}
