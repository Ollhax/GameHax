using System;

namespace MG.Framework.Input
{
	[Flags]
	enum KeyModifier
	{
		None = 0,
		Any = 1 << 0,
		Ctrl = 1 << 1,
		Shift = 1 << 2,
		Alt = 1 << 3,
	}
}
