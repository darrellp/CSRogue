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
            var reader = new StringReader(mapString);
			var excavator = new FileExcavator(reader, null);
			var csRogueMap = new CsRogueMap();
			excavator.Excavate(csRogueMap);

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
			Test(mapString, new MapCoordinates(0, 0), 5, 37);

			mapString =
@"...............
.#.............
...............
...............
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(0, 0), 4, 23);

			mapString =
@"...............
...............
...............
...............
...............
...............
...............
...............";
			var fov = Test(mapString, new MapCoordinates(0, 0), 4, 26);
			fov.Scan(new MapCoordinates(1, 0));
			Assert.AreEqual(6, fov.NewlySeen.ToList().Count);
			Assert.AreEqual(1, fov.NewlyUnseen.ToList().Count);
			Test(mapString, new MapCoordinates(14, 7), 4, 26);
			Test(mapString, new MapCoordinates(0, 7), 4, 26);
			Test(mapString, new MapCoordinates(14, 0), 4, 26);
			Test(mapString, new MapCoordinates(7, 3), 4, 49);

			mapString =
@"...#...........
...............
...#...........
...............
...............
...............
...............
...............";
			Test(mapString, new MapCoordinates(0, 0), 4, 25);
			Test(mapString, new MapCoordinates(0, 0), 5, 34);

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
