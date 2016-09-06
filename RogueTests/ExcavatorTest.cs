using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueTests
{
	public class Sword : Item
	{
		public Sword(Level l) { }
	}

	public class Person : Creature
	{
		public Person(Level l) { }
		public override bool IsPlayer => true;
	}

	public class Rat : Creature
	{
		public Rat(Level l) { }
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

		readonly Guid _personId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
		readonly Guid _ratId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

	    /// <summary>
        ///A test for Excavate
        ///</summary>
        [TestMethod()]
        public void ExcavateTest()
        {
		const string input = @"
//type										ch	name		weight	value	description								class
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player								RogueTests.Person
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.				RogueTests.Rat
																			These rodents seem to be everywhere!
36BC6779-846D-4949-8F30-7C5F97E5E729		!	Sword		10		15		A sharp, pointy, hurty thingy			RogueTests.Sword
-----------------------------------------------------------------------------------------------------------------------------------------
//name  type										HP		Lvl	Rarity	Color	Speed	AC
player	0D583F58-FA20-4292-A272-37919917644A		2d6		.	.		.		.		8
rat		1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		2d6		0	2		Red		.		3
";
		const string mapString =
@"@r-............
...............
...............
...............";

			TextReader reader = new StringReader(input);
            StringReader stream = new StringReader(mapString);
            FileExcavator excavator = new FileExcavator(stream, new ItemFactory(reader));
            CsRogueMap csRogueMap = new CsRogueMap();
            excavator.Excavate(csRogueMap);
            CheckLocation(csRogueMap, 0, 0, TerrainType.Floor, _personId);
            CheckLocation(csRogueMap, 1, 0, TerrainType.Floor, _ratId);
            CheckLocation(csRogueMap, 2, 0, TerrainType.HorizontalWall);
            CheckLocation(csRogueMap, 3, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 14, 0, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 0, TerrainType.OffMap);
            CheckLocation(csRogueMap, 14, 3, TerrainType.Floor);
            CheckLocation(csRogueMap, 15, 4, TerrainType.OffMap);
        }
    }
}
