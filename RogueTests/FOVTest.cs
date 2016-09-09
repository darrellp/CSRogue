using System.IO;
using System.Linq;
using System.Text;
using CSRogue.Map_Generation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for FOVTest and is intended
	///to contain all FOVTest Unit Tests
	///</summary>
	[TestClass()]
	public class FOVTest
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


		public FOV Test(string mapString, MapCoordinates location, int maxRow, int countSeen)
		{
			var csRogueMap = new BaseMap(mapString, Initialization.ItemFactory);

			var fov = new FOV(csRogueMap, maxRow);
			fov.Scan(location);
			Assert.AreEqual(countSeen, fov.NewlySeen.ToList().Count);
			return fov;
		}

		/// <summary>
		///A test for NewlySeen
		///</summary>
		[TestMethod()]
		public void NewlySeenTest()
		{
			var mapString = @"...............
...............
...............
...#...........
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(0, 0), 5, 26);

			mapString =
@"...............
.#.............
...............
...............
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(0, 0), 4, 14);

			mapString =
@"...............
...............
...............
...............
...............
...............
...............
...............";
			var fov = Test(mapString, new MapCoordinates(0, 0), 4, 17);
			fov.Scan(new MapCoordinates(1, 0));
			Assert.AreEqual(5, fov.NewlySeen.ToList().Count);
			Assert.AreEqual(1, fov.NewlyUnseen.ToList().Count);
			Test(mapString, new MapCoordinates(14, 7), 4, 17);
			Test(mapString, new MapCoordinates(0, 7), 4, 17);
			Test(mapString, new MapCoordinates(14, 0), 4, 17);
			Test(mapString, new MapCoordinates(7, 3), 4, 48);

			mapString =
@"...#...........
...............
...#...........
...............
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(0, 0), 4, 16);
			Test(mapString, new MapCoordinates(0, 0), 5, 24);

			mapString =
@"...............
...............
.......#.......
...............
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(7, 3), 4, 44);
		}
	}
}
