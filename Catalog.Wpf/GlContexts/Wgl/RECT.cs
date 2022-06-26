﻿using System.Runtime.InteropServices;

namespace Catalog.Wpf.GlContexts.Wgl
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
}
