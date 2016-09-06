using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSRogue.Map_Generation;
using static RogueTests.TestFactory;

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

        void CheckLocation(CsRogueMap csRogueMap, int iCol, int iRow, TerrainType terrain, Guid itemId = default(Guid))
        {
            if (itemId != default(Guid))
            {
                Assert.AreEqual(1, csRogueMap.Items(iCol, iRow).Count);
                Assert.AreEqual(itemId, csRogueMap.Items(iCol, iRow)[0].ItemTypeId);
            }
            else
            {
                Assert.AreEqual(0, csRogueMap.Items(iCol, iRow).Count);
            }
            Assert.AreEqual(terrain, csRogueMap.Terrain(iCol, iRow));
        }

	    /// <summary>
        ///A test for Excavate
        ///</summary>
        [TestMethod()]
        public void ExcavateTest()
        {
            const string mapString =
@"@r-............
...............
...............
...............";

            StringReader stream = new StringReader(mapString);
            FileExcavator excavator = new FileExcavator(stream, new TestFactory());
            CsRogueMap csRogueMap = new CsRogueMap();
            excavator.Excavate(csRogueMap);
            CheckLocation(csRogueMap, 0, 0, TerrainType.Floor, PersonId);
            CheckLocation(csRogueMap, 1, 0, TerrainType.Floor, RatId);
            CheckLocation(csRogueMap, 2, 0, TerrainType.HorizontalWall);
            CheckLocation(csRogueMap, 3, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 14, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 0, TerrainType.OffMap);
            CheckLocation(csRogueMap, 14, 3, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 4, TerrainType.OffMap);
        }
    }
}
