using System.Text;
using CSRogue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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

		void CheckLocation(Map map, int iCol, int iRow, TerrainType terrain, ItemType itemType = ItemType.Nothing)
		{
			if (itemType != ItemType.Nothing)
			{
				Assert.AreEqual(1, map.Items(iCol, iRow).Count);
				Assert.AreEqual(itemType, map.Items(iCol, iRow)[0].ItemType);
			}
			else
			{
				Assert.AreEqual(0, map.Items(iCol, iRow).Count);
			}
			Assert.AreEqual(terrain, map.Terrain(iCol, iRow));
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
			MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(mapString));
			FileExcavator excavator = new FileExcavator(stream);
			Map map = new Map();
			excavator.Excavate(map);
			CheckLocation(map, 0, 0, TerrainType.Floor, ItemType.Player);
			CheckLocation(map, 1, 0, TerrainType.Floor, ItemType.Rat);
			CheckLocation(map, 2, 0, TerrainType.HorizontalWall);
			CheckLocation(map, 3, 0, TerrainType.Floor);
			CheckLocation(map, 14, 0, TerrainType.Floor);
			CheckLocation(map, 15, 0, TerrainType.OffMap);
			CheckLocation(map, 14,3, TerrainType.Floor);
			CheckLocation(map, 15, 4, TerrainType.OffMap);
		}

		/// <summary>
		///A test for ExcavateByGrid
		///</summary>
		[TestMethod()]
		public void ExcavateByGridTest()
		{
			Map map1 = new Map();
			GridExcavator excavator = new GridExcavator(seed : 0);
			excavator.Excavate(map1);
			string str1 = map1.ToString();
			Map map2 = new Map();
			excavator.Excavate(map2);
			string str2 = map2.ToString();
			Assert.AreEqual(str1, str2);
		}
	}
}
