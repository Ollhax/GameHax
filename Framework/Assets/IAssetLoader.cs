using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MG.Framework.Assets
{
	public interface IAssetLoader
	{
		Type GetAssetType();
		object Create();
		void Load(object asset, string filePath);
	}
}
