using CSRogue;
using CSRogue.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RogueTests
{
	
	
	/// <summary>
	///This is a test class for DieRollTest and is intended
	///to contain all DieRollTest Unit Tests
	///</summary>
	[TestClass()]
	public class DieRollTest
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
		///A test for DieRoll Constructor
		///</summary>
		[TestMethod()]
		public void DieRollConstructorTest()
		{
			const int dieCount = 2;
			const int dieSides = 6;
			DieRoll target = new DieRoll(dieCount, dieSides);
			Assert.AreEqual(dieCount, target.DieCount);
			Assert.AreEqual(dieSides, target.DieSides);
		}

		/// <summary>
		///A test for DieRoll Constructor
		///</summary>
		[TestMethod()]
		public void DieRollConstructorTest1()
		{
			string roll = "2d6";
			DieRoll target = new DieRoll(roll);
			Assert.AreEqual(2, target.DieCount);
			Assert.AreEqual(6, target.DieSides);
		}

		/// <summary>
		///A test for Roll
		///</summary>
		[TestMethod()]
		public void RollTest()
		{
			DieRoll target = new DieRoll("2d6");
			Rnd.SetGlobalSeed(0);
			for (int i = 0; i < 1000; i++)
			{
				int roll = target.Roll();
				Assert.IsTrue(roll >= 2 && roll <= 12);
			}
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			string roll = "2d6";
			DieRoll target = new DieRoll(roll); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
		    var actual = target.ToString();
			Assert.AreEqual(roll, actual);
		}
	}
}
