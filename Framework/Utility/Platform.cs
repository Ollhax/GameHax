using System;
using System.Runtime.InteropServices;

namespace MG.Framework.Utility
{
	/// <summary>
	/// Tools for determining which platform we are running on.
	/// </summary>
	public static class Platform
	{
		/// <summary>
		/// Running on Windows?
		/// </summary>
		public static bool IsWindows { get; private set; }

		/// <summary>
		/// Running on Mac?
		/// </summary>
		public static bool IsMac { get; private set; }

		/// <summary>
		/// Running on Unix?
		/// </summary>
		public static bool IsX11 { get; private set; }
		
		static Platform()
		{
			IsWindows = System.IO.Path.DirectorySeparatorChar == '\\';
			IsMac = !IsWindows && IsRunningOnMac();
			IsX11 = !IsMac && Environment.OSVersion.Platform == PlatformID.Unix;
		}

		//From Managed.Windows.Forms/XplatUI
		[DllImport("libc")]
		private static extern int uname(IntPtr buf);

		private static bool IsRunningOnMac()
		{
			var buffer = IntPtr.Zero;

			try
			{
				buffer = Marshal.AllocHGlobal(8192);

				if (uname(buffer) == 0)
				{
					var os = Marshal.PtrToStringAnsi(buffer);
					
					if (os == "Darwin")
					{
						return true;
					}
				}
			}
			catch
			{

			}
			finally
			{
				if (buffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
			return false;
		}
	}
}