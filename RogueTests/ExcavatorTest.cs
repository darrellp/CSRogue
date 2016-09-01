using System;
using System.Collections.Generic;
using System.Text;
using CSRogue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Remoting.Messaging;
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

        static Guid personId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
        static Guid ratId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

        class Person : IItem
	    {
	        public Guid ItemTypeId { get; } = personId;
	        public MapCoordinates Location { get; set; }
	        public char Ch { get; set; } = '@';
	    }

        class Rat : IItem
        {
            public Guid ItemTypeId { get; } = ratId;
            public MapCoordinates Location { get; set; }
            public char Ch { get; set; } = 'r';
        }

	    static Dictionary<Guid, Func<IItem>> GuidToCreator = new Dictionary<Guid, Func<IItem>>()
        {
	        {ratId, () => new Rat()},
            {personId, () => new Person() }
	    };

        class TestFactory : IItemFactory
	    {
	        public Dictionary<Guid, ItemInfo> InfoFromId { get; }
	        public IItem Create(Guid id, Level level)
	        {
	            return GuidToCreator[id]();
	        }

	        public TestFactory()
	        {
                const string input = @"
//type										ch	name		weight	value	description
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.
																			These rodents seem to be everywhere!
";

                TextReader reader = new StringReader(input);
                InfoFromId = ReadItemData.GetData(reader);
            }
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
            CheckLocation(csRogueMap, 0, 0, TerrainType.Floor, personId);
            CheckLocation(csRogueMap, 1, 0, TerrainType.Floor, ratId);
            CheckLocation(csRogueMap, 2, 0, TerrainType.HorizontalWall);
            CheckLocation(csRogueMap, 3, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 14, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 0, TerrainType.OffMap);
            CheckLocation(csRogueMap, 14, 3, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 4, TerrainType.OffMap);
        }
    }
}
