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
#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
#endif
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

			MessageBox.Show("Oops, something went very wrong! " + "Information stored in \"" + exceptionFile + "\". Please send it to ollhak@gmail.com!\n\nError: " + exceptionText,
			  "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop);

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
