using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	public class RenderContext
	{
		public Screen ActiveScreen { get; private set; }
		public QuadBatch QuadBatch { get; private set; }
		public PrimitiveBatch PrimitiveBatch { get; private set; }
		public TextureFill TextureFill { get; private set; }
		
		public Matrix DefaultProjection;
		
		public RenderContext()
		{
			QuadBatch = new QuadBatch();
			PrimitiveBatch = new PrimitiveBatch();
			TextureFill = new TextureFill();
			DefaultProjection = Matrix.Identity;
		}

		public void Prepare(Screen screen)
		{
			ActiveScreen = screen;

			var size = screen.ScreenSize;
			DefaultProjection = Matrix.CreateOrthographicOffCenter(0, size.X, size.Y, 0, -1024, 1024);

			QuadBatch.CurrentContext = this;
			PrimitiveBatch.CurrentContext = this;
			TextureFill.CurrentContext = this;
		}
	}
}
