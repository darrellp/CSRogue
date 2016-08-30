using System.Text;
using CSRogue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for ExcavatorTest and is intended
	///to contain all ExcavatorTest Unit Tests
	///</summary>
	[TestClass()]
	public class ExcavatorTest
	{
	    /// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

	    #region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
		}

		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for ExcavateByGrid
		///</summary>
		[TestMethod()]
		public void ExcavateByGridTest()
		{
			CsRogueMap map1 = new CsRogueMap();
			GridExcavator excavator = new GridExcavator(seed : 0);
			excavator.Excavate(map1);
			string str1 = map1.ToString();
			CsRogueMap map2 = new CsRogueMap();
			excavator.Excavate(map2);
			string str2 = map2.ToString();
			Assert.AreEqual(str1, str2);
		}
	}
}
