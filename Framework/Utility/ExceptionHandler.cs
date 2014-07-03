using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace MG.Framework.Utility
{
	public static class ExceptionHandler
	{
		public static void Initialize()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
		}

		public static void RaiseException(Exception ex, bool close)
		{
			if (ex.InnerException != null)
			{
				RaiseException(ex.InnerException, close);
				return;
			}

			string exceptionText = ex.Message;

			var fullExceptionText = exceptionText
				+ Environment.NewLine
				+ "--------------" + Environment.NewLine
				+ "Version: " + Assembly.GetEntryAssembly().GetName().Version + Environment.NewLine
				+ "Stacktrace: " + Environment.NewLine
				+ ex.StackTrace;

			string exceptionFile = "exception.txt";
			using (var writer = new StreamWriter(exceptionFile))
			{
				writer.WriteLine(fullExceptionText);
			}

#if !DEBUG
			MessageBox.Show("Oops, something went very wrong! " + "Information stored in \"" + exceptionFile + "\". Please send it to ollhak@gmail.com!\n\nError: " + exceptionText,
			  "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop);
#else			
			Debug.WriteLine(fullExceptionText);
			System.Diagnostics.Debugger.Break();
#endif

			if (close)
			{
				Framework.Deinitialize();
			}
		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			RaiseException((Exception)unhandledExceptionEventArgs.ExceptionObject, true);
		}
	}
}
