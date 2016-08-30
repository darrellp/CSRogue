using CSRogue.Item_Handling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for ReadItemDataTest and is intended
	///to contain all ReadItemDataTest Unit Tests
	///</summary>
	[TestClass()]
	public class ReadItemDataTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
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
		///A test for GetData
		///</summary>
		[TestMethod()]
		public void GetDataTest()
		{
		    var input = @"
//type										ch	name		weight	value	description
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.
																			These rodents seem to be everywhere!
";
		    TextReader reader = new StringReader(input);
            Dictionary<Guid, ItemInfo> infoList = ReadItemData.GetData(reader);
            var heroId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
            var ratId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");
			TestItemInfo(infoList[heroId], name:"Player", itemId:heroId, character:'@', description:"The Player");
			TestItemInfo(infoList[ratId], name:"Rat", itemId:ratId, character:'r', description:"A vile little sewer rat. These rodents seem to be everywhere!");
		}
	}
}
