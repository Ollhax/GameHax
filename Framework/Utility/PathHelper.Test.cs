using NUnit.Framework;

namespace MG.Framework.Utility
{
	[TestFixture]
	class PathHelperTest
	{
		[Test]
		public void TestGetRelativePath()
		{
			Assert.AreEqual(@"", PathHelper.GetRelativePath(@"c:\meep", @"c:\meep"));
			Assert.AreEqual(@"beep.txt", PathHelper.GetRelativePath(@"c:\meep.txt", @"c:\beep.txt"));
			Assert.AreEqual(@"..\beep.txt", PathHelper.GetRelativePath(@"c:\meep", @"c:\beep.txt"));
			Assert.AreEqual(@"..\agda\schme.gaf", PathHelper.GetRelativePath(@"c:\meep\beep\gamma\nero.txt", @"c:\meep\beep\agda\schme.gaf"));
			Assert.AreEqual(@"..\..\agda\schme", PathHelper.GetRelativePath(@"c:\meep\beep\gamma\nero", @"c:\meep\beep\agda\schme"));
			Assert.AreEqual(@"..\..\..\gamma.fuff", PathHelper.GetRelativePath(@"meep\beep\gamma\nero.txt", @"gamma.fuff"));
			Assert.AreEqual(@"delta", PathHelper.GetRelativePath(@"c:\meep\beep\gamma\nero", @"c:\meep\beep\gamma\nero\delta"));
			Assert.AreEqual(@"delta", PathHelper.GetRelativePath(@"c:\meep\beep\gamma\nero\", @"c:\meep\beep\gamma\nero\delta"));
			Assert.AreEqual(@"delta\omega", PathHelper.GetRelativePath(@"c:\meep\beep\gamma\nero", @"c:\meep\beep\gamma\nero\delta\omega"));
		}
	}
}