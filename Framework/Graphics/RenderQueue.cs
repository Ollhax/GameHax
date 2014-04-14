using System;
using System.Collections.Generic;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// The RenderQueue coordiantes multiple renderers (e.g. QuadBatch, PrimitiveBatch, possibly customized derivates).
	/// The main purpose is to allow drawing objects of different types (i.e. requiring different render settings) all in once bunch,
	/// getting the drawing done in the right order while keeping down the code complexity.
	/// </summary>
	public static class RenderQueue
	{
		public abstract class Batch
		{
			public Action<Batch> Render;
		}

		private static Batch currentBatch;
		private static List<Batch> batches = new List<Batch>();
		
		/// <summary>
		/// Fetch the currently active batch.
		/// </summary>
		public static Batch CurrentBatch { get { return currentBatch; } }
		
		/// <summary>
		/// Flush the active batches, performing the actual graphical rendering.
		/// </summary>
		public static void Flush()
		{
			currentBatch = null;

			foreach (var batch in batches)
			{
				batch.Render(batch);
			}

			batches.Clear();
		}

		/// <summary>
		/// Add a batch to the list. Will also set it as CurrentBatch.
		/// </summary>
		/// <param name="batch">Batch to add.</param>
		public static void Add(Batch batch)
		{
			batches.Add(batch);
			currentBatch = batch;
		}
	}
}
