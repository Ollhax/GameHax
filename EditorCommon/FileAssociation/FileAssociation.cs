using System;
using System.Text;

using MG.Framework.Utility;

using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace MG.EditorCommon.FileAssociation
{
	public class FileAssociation
	{
		[DllImport("Kernel32.dll")]
		private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

		private static string basePath = "Software\\Classes\\";

		/// <summary>
		/// Register the specified file association.
		/// </summary>
		/// <param name="extension">Extention to register, e.g. ".txt".</param>
		/// <param name="progId">The program id to register, e.g. "Class.ProgID".</param>
		/// <param name="description">File type description to register.</param>
		/// <param name="icon">Icon file to register (.ico).</param>
		/// <param name="application">Application file to register.</param>
		public static void Associate(string extension, string progId, string description, string icon, string application)
		{
			if (!Platform.IsWindows) return; // TODO: Fix on other platforms

			using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(basePath + extension))
			{
				registryKey.SetValue("", progId);

				if (!string.IsNullOrEmpty(progId))
				{
					using (RegistryKey key = Registry.CurrentUser.CreateSubKey(basePath + progId))
					{
						if (description != null) key.SetValue("", description);
						if (icon != null) key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
						if (application != null) key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(application) + " \"%1\"");
						
						// Notify explorer of change
						SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
					}
				}
			}
		}

		/// <summary>
		/// Check if the specified extension has any file associations.
		/// </summary>
		/// <param name="extension">Extension to theck, e.g. ".txt".</param>
		/// <returns>True if the specified extension has any file associations.</returns>
		public static bool IsAssociated(string extension)
		{
			if (!Platform.IsWindows) return false; // TODO: Fix on other platforms

			return (Registry.CurrentUser.OpenSubKey(basePath + extension, false) != null);
		}
		
		private static string ToShortPathName(string longName)
		{
			StringBuilder s = new StringBuilder(1000);
			uint iSize = (uint)s.Capacity;
			uint iRet = GetShortPathName(longName, s, iSize);
			return s.ToString();
		}
	}
}
