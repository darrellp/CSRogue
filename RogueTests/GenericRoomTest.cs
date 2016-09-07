using CSRogue.Map_Generation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CSRogue.Utilities;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for GenericRoomTest and is intended
	///to contain all GenericRoomTest Unit Tests
	///</summary>
	[TestClass()]
	public class GenericRoomTest
	{
		private TestContext testContextInstance;

		const string _layout4X2 =
@"....
....";

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
		///A test for GenericRoom Constructor
		///</summary>
		[TestMethod()]
		public void GenericRoomConstructorTest()
		{
			string layout =
@"....
....";

			List<Room> exits = null;
			MapCoordinates location = new MapCoordinates();
			Room target = new Room(layout, location, exits);
			Assert.AreEqual(2, target.Height());
			Assert.AreEqual(4, target.Width());
		}

		/// <summary>
		///A test for CombineWith
		///</summary>
		[TestMethod()]
		public void CombineWithTest()
		{
			string layoutConnect =
@"........
........
        ";
			Room roomConnect = new Room(layoutConnect, new MapCoordinates(10, 7), null);
			string layout1 =
@"    
....
....";
			Room room1 = new Room(layout1, new MapCoordinates(10, 9), null);
			Room room2 = new Room(layout1, new MapCoordinates(14, 9), null);
			roomConnect.AddExit(room1, new MapCoordinates(10, 9));
			roomConnect.AddExit(room2, new MapCoordinates(14, 9));
			room1.AddExit(roomConnect, new MapCoordinates(10, 9));
			room2.AddExit(roomConnect, new MapCoordinates(14, 9));
			room1.CombineWith(room2);
			Assert.AreEqual(2, room1.ExitMap.Count);
			Assert.AreEqual(8, room1.Width());
			Assert.AreEqual(3, room1.Height());
			Assert.IsTrue(room1.ExitMap.ContainsKey(new MapCoordinates(10, 9)));
			Assert.IsTrue(room1.ExitMap.ContainsKey(new MapCoordinates(14, 9)));
			Assert.AreSame(roomConnect, room1.ExitMap[new MapCoordinates(10, 9)]);
			Assert.AreSame(roomConnect, room1.ExitMap[new MapCoordinates(14, 9)]);
		}

		/// <summary>
		///A test for MarkTerrain
		///</summary>
		[TestMethod()]
		public void MarkTerrainTest()
		{
			Room target = new Room(_layout4X2, new MapCoordinates(10, 10), null);
			MapCoordinates mapLocation = new MapCoordinates(10, 10);
			target[mapLocation] = '#';
			Assert.AreEqual('#', target[mapLocation]);
		}
	}
}
