using CSRogue.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CSRogue.Map_Generation;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for BresenhamStepperTest and is intended
	///to contain all BresenhamStepperTest Unit Tests
	///</summary>
	[TestClass()]
	public class BresenhamStepperTest
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
		///A test for BresenhamStepper Constructor
		///</summary>
		[TestMethod()]
		public void BresenhamStepperConstructorTest()
		{
			TestBresenhamConstruction(0, 0, 1, 1);
			TestBresenhamConstruction(0, 0, 0, 0, expectException:true);
			TestBresenhamConstruction(0, 0, 1, 1, startingColumn:5);
			TestBresenhamConstruction(0, 0, 1, 1, startingRow: 5);
			TestBresenhamConstruction(0, 0, 1, 1, startingColumn: -3, expectException: true);
			TestBresenhamConstruction(0, 0, 1, 0, startingRow: 1, expectException: true);
			TestBresenhamConstruction(0, 0, 1, 0, startingColumn: 1);
			TestBresenhamConstruction(0, 0, 1, 0, startingColumn: -1, expectException: true);
			TestBresenhamConstruction(0, 0, 1, 1, startingColumn: 0, startingRow: 0, expectException: true);
		}

		private bool TestBresenhamConstruction(
			int columnStart,
			int rowStart,
			int columnEnd,
			int rowEnd,
			int? startingColumn = null,
			int? startingRow = null,
			bool expectException = false)
		{
			bool threwException = false;

			try
			{
				MapCoordinates start = new MapCoordinates(columnStart, rowStart);
				MapCoordinates end = new MapCoordinates(columnEnd, rowEnd);
				BresenhamStepper stepper = new BresenhamStepper(start, end, startingColumn, startingRow);
			}
			catch (Exception)
			{
				threwException = true;
			}
			return !(!expectException && threwException || expectException && !threwException);
		}

		/// <summary>
		///A test for Steps
		///</summary>
		[TestMethod()]
		public void StepsTest()
		{
			Random rnd = new Random(0);
			MapCoordinates offset = new MapCoordinates(50, 50);
			for (int iTest = 0; iTest < 200; iTest++)
			{
				MapCoordinates loc1 = new MapCoordinates(rnd.Next(), rnd.Next());
				MapCoordinates diff = new MapCoordinates(rnd.Next(100), rnd.Next(100)) - offset;
				if (rnd.Next(100) < 10)
				{
					diff = new MapCoordinates(diff.Column, 0);
				}
				if (rnd.Next(100) < 10)
				{
					diff = new MapCoordinates(0, diff.Row);
				}
				if (diff.Row == 0 && diff.Column == 0)
				{
					continue;
				}
				MapCoordinates loc2 = loc1 + diff;

				BresenhamStepper stepper = new BresenhamStepper(loc1, loc2);
				var steps = stepper.Steps.GetEnumerator();
			    bool foundLast = false;
				steps.MoveNext();

				var foundFirst = steps.Current == loc1;

				for (int iStep = 0; iStep < 51; iStep++)
				{
					foundLast = steps.Current == loc2;
					if (foundLast)
					{
						break;
					}
					steps.MoveNext();
				}
				Assert.IsTrue(foundFirst);
				Assert.IsTrue(foundLast);
			}
		}
	}
}
