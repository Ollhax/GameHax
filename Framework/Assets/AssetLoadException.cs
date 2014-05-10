using System;

namespace MG.Framework.Assets
{
	class AssetLoadException : Exception
	{
		public AssetLoadException(string error, Exception exception)
			: base(error, exception)
		{
			
		}
	}
}
