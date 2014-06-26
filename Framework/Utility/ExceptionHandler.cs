using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MG.Framework.Utility
{
	static class ExceptionHandler
	{
		public static void Initialize()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			Exception ex = (Exception)unhandledExceptionEventArgs.ExceptionObject;
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

			MessageBox.Show("Oops, something went very wrong! " + "Information stored in \"" + exceptionFile + "\". Please send it to ollhak@gmail.com!\n\nError:\"" + exceptionText + "\"",
			  "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop);

			Framework.Deinitialize();
		}
	}
}
