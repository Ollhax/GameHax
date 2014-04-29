using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	public class RenderContext
	{
		public Screen ActiveScreen { get; private set; }
		public QuadBatch QuadBatch { get; private set; }
		public PrimitiveBatch PrimitiveBatch { get; private set; }
		public TextureFill TextureFill { get; private set; }
		
		public RenderContext()
		{
			QuadBatch = new QuadBatch();
			PrimitiveBatch = new PrimitiveBatch();
			TextureFill = new TextureFill();
		}

		public void Prepare(Screen screen)
		{
			ActiveScreen = screen;

			QuadBatch.CurrentContext = this;
			PrimitiveBatch.CurrentContext = this;
			TextureFill.CurrentContext = this;
		}
	}
}
