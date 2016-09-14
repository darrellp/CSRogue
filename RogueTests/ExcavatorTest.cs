using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueTests
{
	public class Sword : Item
	{
		public Sword(Level l) { }
	}

	public class Person : Creature, IPlayer
	{
		public Person(Level l) { }
	}

	public class Rat : Creature
	{
		public Rat(Level l) { }
	}

    public class Orc : Creature
    {
        public Orc(Level l) { }
    }

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
            Rnd.SetGlobalSeed(0);
			BaseMap map1 = new BaseMap(100, 100);
			GridExcavator excavator = new GridExcavator();
			excavator.Excavate(map1);
			string str1 = map1.ToString();
            BaseMap map2 = new BaseMap(100, 100);
			excavator.Excavate(map2);
			string str2 = map2.ToString();
			Assert.AreEqual(str1, str2);
        }

        void CheckLocation(BaseMap baseMap, int iCol, int iRow, TerrainType terrain, Guid itemId = default(Guid))
        {
            if (itemId != default(Guid))
            {
                Assert.AreEqual(1, baseMap[iCol, iRow].Items.Count);
                Assert.AreEqual(itemId, baseMap[iCol, iRow].Items[0].ItemTypeId);
            }
            else
            {
                Assert.IsTrue(baseMap[iCol, iRow].Items == null || baseMap[iCol, iRow].Items.Count == 0);
            }
            Assert.AreEqual(terrain, baseMap[iCol, iRow].Terrain);
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

            BaseMap baseMap = new BaseMap(mapString, Initialization.ItemFactory);
            CheckLocation(baseMap, 0, 0, TerrainType.Floor, Initialization.PersonId);
            CheckLocation(baseMap, 1, 0, TerrainType.Floor, Initialization.RatId);
            CheckLocation(baseMap, 2, 0, TerrainType.HorizontalWall);
            CheckLocation(baseMap, 3, 0, TerrainType.Floor);
            CheckLocation(baseMap, 14, 0, TerrainType.Floor);
            CheckLocation(baseMap, 14, 3, TerrainType.Floor);
        }
    }
}
