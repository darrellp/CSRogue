using CSRogue.Item_Handling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
			ItemType itemType = ItemType.Nothing,
			string name = null,
			char character = ' ',
			double weight = 0,
			int value = 0,
			string description = "A singularly uninteresting item")
		{
			Assert.AreEqual(itemType, info.ItemType);
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
			Dictionary<ItemType, ItemInfo> infoList = ReadItemData.GetData();
			TestItemInfo(infoList[ItemType.Player], name:"Player", itemType:ItemType.Player, character:'@', description:"The Player");
			TestItemInfo(infoList[ItemType.Rat], name:"Rat", itemType:ItemType.Rat, character:'r', description:"A vile little sewer rat. These rodents seem to be everywhere!");
		}
	}
}
