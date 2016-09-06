using CSRogue.Item_Handling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueTests
{
	/// <summary>
	///This is a test class for ReadItemDataTest and is intended
	///to contain all ReadItemDataTest Unit Tests
	///</summary>
	[TestClass()]
	public class ReadItemDataTest
	{
        static readonly Guid HeroId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
        static readonly Guid RatId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

		private TestContext _testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
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
        ///A test for GetData
        ///</summary>
        [TestMethod()]
		public void GetDataTest()
		{
		    var input = @"
//type										ch	name		weight	value	description								class
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player								RogueTests.Person
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.				RogueTests.Rat
																			These rodents seem to be everywhere!
";
		    TextReader reader = new StringReader(input);
            Dictionary<Guid, ItemInfo> infoList = (new ReadItemData()).GetData(reader);
			TestItemInfo(infoList[HeroId], name:"Player", itemId:HeroId, character:'@', description:"The Player");
			TestItemInfo(infoList[RatId], name:"Rat", itemId:RatId, character:'r', description:"A vile little sewer rat. These rodents seem to be everywhere!");
		}

        void TestItemInfo(
            ItemInfo info,
            Guid itemId = default(Guid),
            string name = null,
            char character = ' ',
            double weight = 0,
            int value = 0,
            string description = "A singularly uninteresting item")
        {
            Assert.AreEqual(itemId, info.ItemId);
            Assert.AreEqual(name, info.Name);
            Assert.AreEqual(character, info.Character);
            Assert.AreEqual(weight, info.Weight);
            Assert.AreEqual(value, info.Value);
            Assert.AreEqual(description, info.Description);
        }

        /// <summary>
        ///A test for ReadCreatureData
        ///</summary>
        [TestMethod()]
        public void ReadCreatureDataTest()
        {
            var input = @"
//name  type										HP		Lvl	Rarity	Color	Speed	AC
player	0D583F58-FA20-4292-A272-37919917644A		2d6		.	.		.		.		8
rat		1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		2d6		0	2		Red		.		3
";
            TextReader reader = new StringReader(input);
            Dictionary<Guid, CreatureInfo> infoList = ReadCreatureData.GetData(reader);
            TestCreatureData(infoList[HeroId], hp: new DieRoll("2d6"), ac: 8);
            TestCreatureData(infoList[RatId], hp: new DieRoll("2d6"), level: 0, rarity: 2, color: RogueColor.Red, ac: 3);
        }

        void TestCreatureData(
            CreatureInfo info,
            DieRoll hp = null,
            int level = 0,
            int rarity = 1,
            RogueColor color = RogueColor.White,
            int speed = 100,
            int ac = 0)
        {
            Assert.AreEqual(hp.DieCount, info.HitPoints.DieCount);
            Assert.AreEqual(hp.DieSides, info.HitPoints.DieSides);
            Assert.AreEqual(level, info.Level);
            Assert.AreEqual(rarity, info.Rarity);
            Assert.AreEqual(color, info.Color);
            Assert.AreEqual(speed, info.Speed);
            Assert.AreEqual(ac, info.ArmorClass);
        }
    }
}
