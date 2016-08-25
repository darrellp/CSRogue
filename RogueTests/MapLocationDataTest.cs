using CSRogue;
using CSRogue.Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSRogue.Item_Handling;
using System.Collections.Generic;
using CSRogue.Map_Generation;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for MapLocationDataTest and is intended
	///to contain all MapLocationDataTest Unit Tests
	///</summary>
	[TestClass()]
	public class MapLocationDataTest
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


		/// <summary>
		///A test for MapLocationData Constructor
		///</summary>
		[TestMethod()]
		public void MapLocationDataConstructorTest()
		{
			TerrainType terrain = TerrainType.VerticalWall;
			List<Item> items = new List<Item>() { new Player(), new Rat() };

			MapLocationData target = new MapLocationData(terrain, items);
			Assert.AreEqual(TerrainType.VerticalWall, target.Terrain);
			Assert.AreEqual(2, target.Items.Count);
			Assert.AreEqual(ItemType.Player, target.Items[0].ItemType);
			Assert.AreEqual(ItemType.Rat, target.Items[1].ItemType);
		}

		/// <summary>
		///A test for MapLocationData Constructor
		///</summary>
		[TestMethod()]
		public void MapLocationDataConstructorTest1()
		{
			MapLocationData target = new MapLocationData();
			Assert.AreEqual(0, target.Items.Count);
			Assert.AreEqual(TerrainType.OffMap, target.Terrain);
		}

		/// <summary>
		///A test for AddItem
		///</summary>
		[TestMethod()]
		public void AddItemTest()
		{
			MapLocationData target = new MapLocationData();
			Item item = new Player();
			target.AddItem(item);
			Assert.AreEqual(1, target.Items.Count);
			Assert.AreEqual(ItemType.Player, target.Items[0].ItemType);
		}

		/// <summary>
		///A test for RemoveItem
		///</summary>
		[TestMethod()]
		public void RemoveItemTest()
		{
			MapLocationData target = new MapLocationData();
			Item item = new Player();
			target.AddItem(item);
			Assert.AreEqual(1, target.Items.Count);
			Assert.AreEqual(ItemType.Player, target.Items[0].ItemType);
			target.RemoveItem(item);
			Assert.AreEqual(0, target.Items.Count);
		}

		/// <summary>
		///A test for FindItemType
		///</summary>
		[TestMethod()]
		public void FindItemTypeTest()
		{
			TerrainType terrain = TerrainType.HorizontalWall;
			List<Item> items = new List<Item>() { new Player(), new Rat() };

			MapLocationData target = new MapLocationData(terrain, items);
			Assert.AreEqual(ItemType.Player, target.FindItemType(ItemType.Player).ItemType);
			Assert.AreEqual(ItemType.Rat, target.FindItemType(ItemType.Rat).ItemType);
		}
	}
}
